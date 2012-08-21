﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fitbit;
using Fitbit.Api;
using System.Configuration;
using Fitbit.Models;

namespace SampleWebMVC.Controllers
{
    public class FitbitController : Controller
    {
        //
        // GET: /Fitbit/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /FitbitAuth/
        // Setup - prepare the user redirect to Fitbit.com to prompt them to authorize this app.
        public ActionResult Authorize()
        {

            //make sure you've set these up in Web.Config under <appSettings>:
            string ConsumerKey = ConfigurationManager.AppSettings["FitbitConsumerKey"];
            string ConsumerSecret = ConfigurationManager.AppSettings["FitbitConsumerSecret"];


            Fitbit.Api.Authenticator authenticator = new Fitbit.Api.Authenticator(ConsumerKey,
                                                                                    ConsumerSecret,
                                                                                    "http://api.fitbit.com/oauth/request_token",
                                                                                    "http://api.fitbit.com/oauth/access_token",
                                                                                    "http://api.fitbit.com/oauth/authorize");
            string authUrl = authenticator.GetAuthUrlToken();


            return Redirect(authUrl);
        }

        //Final step. Take this authorization information and use it in the app
        public ActionResult Callback()
        {
            string OAuthToken = Request.Params["oauth_token"];
            string OAuthVerifier = Request.Params["oauth_verifier"];

            string ConsumerKey = ConfigurationManager.AppSettings["FitbitConsumerKey"];
            string ConsumerSecret = ConfigurationManager.AppSettings["FitbitConsumerSecret"];

            //this is going to go back to Fitbit one last time (server to server) and get the user's permanent auth credentials

            //create the Authenticator object
            Fitbit.Api.Authenticator authenticator = new Fitbit.Api.Authenticator(ConsumerKey,
                                                                                    ConsumerSecret,
                                                                                    "http://api.fitbit.com/oauth/request_token",
                                                                                    "http://api.fitbit.com/oauth/access_token",
                                                                                    "http://api.fitbit.com/oauth/authorize");
            //execute the Authenticator request to Fitbit
            AuthCredential credential = authenticator.ProcessApprovedAuthCallback(OAuthToken, OAuthVerifier);

            //here, we now have everything we need for the future to go back to Fitbit's API (STORE THESE):
            //  credential.AuthToken;
            //  credential.AuthTokenSecret;
            //  credential.UserId;

            // For demo, put this in the session managed by ASP.NET
            Session["FitbitAuthToken"] = credential.AuthToken;
            Session["FitbitAuthTokenSecret"] = credential.AuthTokenSecret;
            Session["FitbitUserId"] = credential.UserId;
            
            return RedirectToAction("Index", "Home");

        }

        public string TestTimeSeries()
        {
            FitbitClient client = new FitbitClient(ConfigurationManager.AppSettings["FitbitConsumerKey"],
                ConfigurationManager.AppSettings["FitbitConsumerSecret"],
                Session["FitbitAuthToken"].ToString(),
                Session["FitbitAuthTokenSecret"].ToString());

            var results = client.GetTimeSeries(TimeSeriesResourceType.ActiveScore, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

            string sOutput = "";
            foreach (var result in results.DataList)
            {
                sOutput += result.DateTime.ToString() + " - " + result.Value.ToString();
            }

            return sOutput;

        }

        public ActionResult LastWeekActiveScore()
        {
            FitbitClient client = new FitbitClient(ConfigurationManager.AppSettings["FitbitConsumerKey"],
                ConfigurationManager.AppSettings["FitbitConsumerSecret"],
                Session["FitbitAuthToken"].ToString(),
                Session["FitbitAuthTokenSecret"].ToString());

            TimeSeriesDataList results = client.GetTimeSeries(TimeSeriesResourceType.ActiveScore, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
            
            return View(results);
        }

        /// <summary>
        /// This requires the Fitbit staff approval of your app before it can be called
        /// </summary>
        /// <returns></returns>
        public string TestIntraDay()
        {
            FitbitClient client = new FitbitClient(ConfigurationManager.AppSettings["FitbitConsumerKey"],
                ConfigurationManager.AppSettings["FitbitConsumerSecret"],
                Session["FitbitAuthToken"].ToString(),
                Session["FitbitAuthTokenSecret"].ToString());

            IntradayData data = client.GetIntraDayTimeSeries(IntradayResourceType.Steps, new DateTime(2012,5,28,11,0,0), new TimeSpan(1,0,0));

            string result = "";

            foreach (IntradayDataValues intraData in data.DataSet)
            {
                result += intraData.Time.ToShortTimeString() + " - " + intraData.Value + Environment.NewLine;
            }

            return result;

        }
    }
}