using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XMachine.Utilities {
    public static class Utilities {

        public static bool isAlphaNumeric(string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            Regex r = new Regex("^(?=.+\\d)(?=.+[a-zA-Z]).*$");
            return r.IsMatch(str);
        }
        public static bool isLettersAndNumber(string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            Regex r = new Regex("^([a-zA-Z0-9])*$");
            return r.IsMatch(str);
        }
        public static bool isMD5(string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            Regex r = new Regex("^[a-f0-9]{32}$");
            return r.IsMatch(str);
        }
        public static bool isHexStr(string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            Regex r = new Regex("^[a-fA-F0-9]+$");
            return r.IsMatch(str);
        }
        public static bool isFQDN(string url) {
            if (string.IsNullOrEmpty(url))
                return false;
            Regex r = new Regex(@"^https?:\/\/(www\.)?[a-zA-Z0-9\\.-_]{1,256}\.[a-zA-z0-9]{1,6}\b([a-zA-Z0-9-()@:%_\\+.~#?&=\/]*)$");
            return r.IsMatch(url);
        }
        public static bool passwordCase(string str) {
            if (string.IsNullOrEmpty(str))
                return false;
            Regex r = new Regex("^(?=.*\\d)(?=.*[A-Z])(?=.*[_.@$!#%^&()_+=]).*$");
            return r.IsMatch(str);
        }

        public static bool isPhone(string phone) {
            if (string.IsNullOrEmpty(phone))
                return false;
            Regex r = new Regex(@"^[07981]{3}[0-9]{8}|234[7981]{2}[0-9]{8}$");
            return r.IsMatch(phone);
        }

        public static string convertIntPhoneToLocal(string phone) {
            var validPhone = "0" + phone[^10..];
            return validPhone;
        }

        public static long getTimeStamp(
                        int year, int month, int day,
                        int hour, int minute, int second, int milliseconds) {
            DateTime value = new DateTime(year, month, day);
            var date = new DateTime(1970, 1, 1, 0, 0, 0, value.Kind);
            var unixTimestamp = Convert.ToInt64((value - date).TotalSeconds);
            return unixTimestamp;
        }
        public static (DateTime modernDate, long unixTimestamp, long unixTimestampMS) getTodayDate() {
            long totalSeconds = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            long totalmSecs = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(totalSeconds).ToLocalTime();
            return (modernDate: dtDateTime, unixTimestamp: totalSeconds, unixTimestampMS: totalmSecs);
        }
        public static DateTime unixTimeStampToDateTime(double unixTimeStamp, bool dateOnly = false) {
            try {
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                if (!dateOnly)
                    return dtDateTime;
                return dtDateTime.Date;
            } catch (Exception) {
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            }
        }

        public static JObject asJObject(object obj) {
            JObject json = JObject.Parse(JsonConvert.SerializeObject(obj));
            return json;
        }
        public static string findString(JObject json, string needle) {
            try {
                return json[needle].ToString();
            } catch {
                return null;
            }
        }
        public static JArray findArray(JObject json, string needle) {
            try {
                return JArray.Parse(json[needle].ToString());
            } catch {
                return null;
            }
        }
        public static JObject findObj(JObject json, string needle) {
            try {
                return JObject.Parse(json[needle].ToString());
            } catch {
                return null;
            }
        }
        public static double? findNumber(JObject json, string needle) {
            try {
                return double.Parse(json[needle].ToString());
            } catch {
                return null;
            }
        }

        public static string validBase64(string raw) {
            int pads = raw.Length % 4;
            if (pads > 0) {
                raw += new string('=', 4 - pads);
            }
            return raw;
        }

        public static bool noNullValue(object obj) {
            foreach (PropertyInfo prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                var data = prop.GetValue(obj);
                if (data == null)
                    return false;
            }
            return true;
        }
        public static byte[] hexStringToByteArray(string input) {
            var outputLength = input.Length / 2;
            var output = new byte[outputLength];
            using (var sr = new StringReader(input)) {
                for (var i = 0; i < outputLength; i++)
                    output[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return output;
        }
        public static string byteArrToHexStr(byte[] byteArr) {
            StringBuilder hex = new StringBuilder(byteArr.Length * 2);
            foreach (byte b in byteArr) hex.AppendFormat("{0:x2}", b); return hex.ToString();
        }
        public static bool noNullValue(object obj, List<string> properties) {
            try {
                foreach (string propertyName in properties) {
                    var data = obj.GetType().GetProperty(propertyName).GetValue(obj, null);
                    if (data == null)
                        return false;
                }
                return true;
            } catch {
                return false;
            }
        }

        public static double ToRadians(this double angleIn10thofaDegree) {
            // Angle in 10th of a degree
            return (angleIn10thofaDegree * Math.PI) / 1800;
        }
        public static string objectToString<T>(T obj) {
            return JObject.FromObject(obj).ToString();
        }
        public static string fromBase64String(string base64str, Encoding encoding = null) {
            byte[] decodedBytes = Convert.FromBase64String(base64str);
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetString(decodedBytes);
        }

        public static string toBase64String(string str, Encoding encoding = null) {
            if (encoding == null)
                encoding = Encoding.UTF8;
            byte[] bytes = encoding.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }
        public static T stringToObject<T>(string dataStr) {
            return JObject.Parse(dataStr).ToObject<T>();
        }

        private static readonly Random getrandom = new Random();
        public static string genID(int counts, int type = 0) {
            string[] strings = new string[3];
            strings[0] = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            strings[1] = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            strings[2] = "1234567890";
            char[] generated = new char[counts];
            char[] characters = strings[type].ToCharArray();
            var random = new Random();
            for (int i = 0; i < counts; i++) {
                int index = random.Next(0, strings[type].Length - 1);
                generated[i] = characters[index];
            }
            return new string(generated);
        }
    }
}
