/*Copyright 2011 Arash Norouzi

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Net;

namespace CommunicationApp.Web.Infrastructure.PushNotificationFile
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // var payload1 = new NotificationPayload("Device token","Message",Badge,"Sound");
            //var payload1 = new NotificationPayload("147e02e7d7bb81cd7fbec9025c78e63d22c0cfdb69cc2384f24374ff1a4d18dc", "Testing Testing",  1, "default");
            //string Message = "{\"flag\":1,\"message\":\"You have a request from bharat\",\"UserTripRequestId\":1,\"RiderId\":1,\"RiderImage\":\" \",\"RiderName\":\" \",\"KmAway\":\"0\",\"MobileNo\":\"978075757\"}";
            ////string Message = "{\"UserTripRequestId\":1,\"RiderId\":1,\"RiderImage\":\" \",\"RiderName\":\" \",\"KmAway\":\"0\",\"MobileNo\":\"978075757\"}";
            ////var payload1 = new NotificationPayload("a17d47384cd8768c4a4def6ef2fd33284a6f3da6ba0e47624ec03f72592a04f3", "You have a request from bharat.", 1, "default");
            ////payload1.AddCustom("flag", "1");
            ////payload1.AddCustom("message", Message);
            

            ////var p = new List<NotificationPayload> {payload1};
            //////var FileName = @"E:\Bharat\Study\Project.MoonAPNS\Project.MoonAPNS\Project.MoonAPNS\ArshAulakh.p12";
            ////var FileName = @"E:\Bharat\Study\Project.MoonAPNS\Project.MoonAPNS\Project.MoonAPNS\ck.p12";
            //////E:\Bharat\Study\Project.MoonAPNS\Project.MoonAPNS\Project.MoonAPNS\ck.p12
            //////var push = new PushNotification(true, FileName, "12345");
            ////var push = new PushNotification(true , FileName, "ameba");
            ////var rejected = push.SendToApple(p);
            ////foreach (var item in rejected)
            ////{
            ////    Console.WriteLine(item);
            ////}
            ////Console.ReadLine();

            
            //string ApplicationId = "a17d47384cd8768c4a4def6ef2fd33284a6f3da6ba0e47624ec03f72592a04f3";
            //string UserMessage = "You have a request from bharat.";

            //string Flag = "1";
            //string JsonMessage = "{\"UserTripRequestId\":1,\"RiderId\":1,\"RiderImage\":\" \",\"RiderName\":\" \",\"KmAway\":\"0\",\"MobileNo\":\"978075757\"}";

           //PushNotificatinAlert.SendPushNotification(ApplicationId, UserMessage, Flag, JsonMessage);
        }
    }
}
