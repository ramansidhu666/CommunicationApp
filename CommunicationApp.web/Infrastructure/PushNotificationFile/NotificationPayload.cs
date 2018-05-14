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
using System.Text;
using Newtonsoft.Json.Linq;
using CommunicationApp.Web.Infrastructure.PushNotificationFile;

namespace CommunicationApp.Web.Infrastructure.PushNotificationFile
{
    public class NotificationPayload
    {
        public NotificationAlert Alert { get; set; }

        public string DeviceToken { get; set; }

        public int? Badge { get; set; }

        public string Sound { get; set; }

        public string ContentAvailable { get; set; }

        internal int PayloadId { get; set; }

        public Dictionary<string, object[]> CustomItems
        {
            get;
            private set;
        }
        public Dictionary<string, Dictionary<string, object>> CustomItemsJson
        {
            get;
            private set;
        }

        public NotificationPayload(string deviceToken)
        {
            DeviceToken = deviceToken;
            Alert = new NotificationAlert();
            CustomItems = new Dictionary<string, object[]>();
            CustomItemsJson = new Dictionary<string, Dictionary<string, object>>();
        }

        public NotificationPayload(string deviceToken, string alert)
        {
            DeviceToken = deviceToken;
            Alert = new NotificationAlert() { Body = alert };
            CustomItems = new Dictionary<string, object[]>();
            CustomItemsJson = new Dictionary<string, Dictionary<string, object>>();
        }

        public NotificationPayload(string deviceToken, string alert, int badge)
        {
            DeviceToken = deviceToken;
            Alert = new NotificationAlert() { Body = alert };
            Badge = badge;
            CustomItems = new Dictionary<string, object[]>();
            CustomItemsJson = new Dictionary<string, Dictionary<string, object>>();
        }

        public NotificationPayload(string deviceToken, string alert, int badge, string sound, string contentAvailable)
        {
            DeviceToken = deviceToken;
            Alert = new NotificationAlert() { Body = alert };
            Badge = badge;
            Sound = sound;
            ContentAvailable = contentAvailable;
            CustomItems = new Dictionary<string, object[]>();
            CustomItemsJson = new Dictionary<string, Dictionary<string, object>>();
        }

        public void AddCustom(string key, params object[] values)
        {
            if (values != null)
                this.CustomItems.Add(key, values);
        }
        public void AddCustomJson(string key, Dictionary<string, object> values)
        {
            if (values != null)
                this.CustomItemsJson.Add(key, values);
        }
        public string ToJson()
        {
            JObject json = new JObject();

            JObject aps = new JObject();

            if (!this.Alert.IsEmpty)
            {
                if (!string.IsNullOrEmpty(this.Alert.Body)
                    && string.IsNullOrEmpty(this.Alert.LocalizedKey)
                    && string.IsNullOrEmpty(this.Alert.ActionLocalizedKey)
                    && (this.Alert.LocalizedArgs == null || this.Alert.LocalizedArgs.Count <= 0))
                {
                    aps["alert"] = new JValue(this.Alert.Body);
                }
                else
                {
                    JObject jsonAlert = new JObject();

                    if (!string.IsNullOrEmpty(this.Alert.LocalizedKey))
                        jsonAlert["loc-key"] = new JValue(this.Alert.LocalizedKey);

                    if (this.Alert.LocalizedArgs != null && this.Alert.LocalizedArgs.Count > 0)
                        jsonAlert["loc-args"] = new JArray(this.Alert.LocalizedArgs.ToArray());

                    if (!string.IsNullOrEmpty(this.Alert.Body))
                        jsonAlert["body"] = new JValue(this.Alert.Body);

                    if (!string.IsNullOrEmpty(this.Alert.ActionLocalizedKey))
                        jsonAlert["action-loc-key"] = new JValue(this.Alert.ActionLocalizedKey);

                    aps["alert"] = jsonAlert;
                }
            }

            if (this.Badge.HasValue)
                aps["badge"] = new JValue(this.Badge.Value);

            if (!string.IsNullOrEmpty(this.Sound))
                aps["sound"] = new JValue(this.Sound);

            if (!string.IsNullOrEmpty(this.ContentAvailable))
                aps["content-available"] = new JValue(this.ContentAvailable);

            json["aps"] = aps;

            foreach (string key in this.CustomItems.Keys)
            {
                if (this.CustomItems[key].Length == 1)
                    json[key] = new JValue(this.CustomItems[key][0]);
                else if (this.CustomItems[key].Length > 1)
                    json[key] = new JArray(this.CustomItems[key]);
            }


            foreach (KeyValuePair<string, Dictionary<string, object>> kvp in this.CustomItemsJson)
            {

                JObject itemJson = new JObject();
                foreach (KeyValuePair<string, object> kv in kvp.Value)
                {
                    itemJson[kv.Key] = new JValue(kv.Value);
                }
                json[kvp.Key] = itemJson;
            }

            string rawString = json.ToString(Newtonsoft.Json.Formatting.None, null);

            StringBuilder encodedString = new StringBuilder();
            foreach (char c in rawString)
            {
                if ((int)c < 32 || (int)c > 127)
                    encodedString.Append("\\u" + String.Format("{0:x4}", Convert.ToUInt32(c)));
                else
                    encodedString.Append(c);
            }
            return rawString;// encodedString.ToString();
        }

        public override string ToString()
        {
            return ToJson();
        }
    }
}
