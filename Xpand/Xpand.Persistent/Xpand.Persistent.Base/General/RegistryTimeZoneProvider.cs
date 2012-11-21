using System;
using System.Collections.Generic;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.Persistent.Base.General {
    public class RegistryTimeZoneProvider {
        static readonly Dictionary<string, TimeZoneId> timeZoneIdTable = CreateTimeZoneIdTable();

        public static string GetRegistryKeyNameByTimeZoneId(TimeZoneId id) {
            foreach (string key in timeZoneIdTable.Keys)
                if (timeZoneIdTable[key] == id)
                    return key;
            return String.Empty;
        }
        public static TimeZoneId GetTimeZoneIdByRegistryKeyName(string key) {
            TimeZoneId result;
            return timeZoneIdTable.TryGetValue(key, out result) ? result : TimeZoneId.Greenwich;
        }
        #region CreateTimeZoneIdTable
        static Dictionary<string, TimeZoneId> CreateTimeZoneIdTable() {
            var result = new Dictionary<string, TimeZoneId>{
                {"Afghanistan Standard Time", TimeZoneId.Afghanistan},
                {"Alaskan Standard Time", TimeZoneId.Alaska},
                {"Arab Standard Time", TimeZoneId.Arab},
                {"Arabian Standard Time", TimeZoneId.Arabian},
                {"Arabic Standard Time", TimeZoneId.Arabic},
                {"Argentina Standard Time", TimeZoneId.Argentina},
                {"Armenian Standard Time", TimeZoneId.Armenia},
                {"Atlantic Standard Time", TimeZoneId.Atlantic},
                {"AUS Central Standard Time", TimeZoneId.CentralAustralian},
                {"AUS Eastern Standard Time", TimeZoneId.EasternAustralian},
                {"Azerbaijan Standard Time", TimeZoneId.Azerbaijan},
                {"Azores Standard Time", TimeZoneId.Azores},
                {"Canada Central Standard Time", TimeZoneId.CentralCanadian},
                {"Cape Verde Standard Time", TimeZoneId.CapeVerde},
                {"Caucasus Standard Time", TimeZoneId.Caucasus},
                {"Cen. Australia Standard Time", TimeZoneId.Adelaide},
                {"Central America Standard Time", TimeZoneId.CentralAmerica},
                {"Central Asia Standard Time", TimeZoneId.CentralAsia},
                {"Central Brazilian Standard Time", TimeZoneId.CentralBrazilian},
                {"Central Europe Standard Time", TimeZoneId.CentralEurope},
                {"Central European Standard Time", TimeZoneId.CentralEuropean},
                {"Central Pacific Standard Time", TimeZoneId.CentralPacific},
                {"Central Standard Time", TimeZoneId.Central},
                {"Central Standard Time (Mexico)", TimeZoneId.CentralMexico},
                {"China Standard Time", TimeZoneId.China},
                {"Dateline Standard Time", TimeZoneId.DateLine},
                {"E. Africa Standard Time", TimeZoneId.EasternAfrica},
                {"E. Australia Standard Time", TimeZoneId.EasternAustralia},
                {"E. Europe Standard Time", TimeZoneId.EasternEurope},
                {"E. South America Standard Time", TimeZoneId.EasternSouthAmerica},
                {"Eastern Standard Time", TimeZoneId.Eastern},
                {"Egypt Standard Time", TimeZoneId.Cairo},
                {"Ekaterinburg Standard Time", TimeZoneId.Ekaterinburg},
                {"Fiji Standard Time", TimeZoneId.Fiji},
                {"FLE Standard Time", TimeZoneId.NorthEurope},
                {"Georgian Standard Time", TimeZoneId.Georgian},
                {"GMT Standard Time", TimeZoneId.Lisbon},
                {"Greenland Standard Time", TimeZoneId.Greenland},
                {"Greenwich Standard Time", TimeZoneId.Greenwich},
                {"GTB Standard Time", TimeZoneId.Balkan},
                {"Hawaiian Standard Time", TimeZoneId.Hawaii},
                {"India Standard Time", TimeZoneId.India},
                {"Iran Standard Time", TimeZoneId.Iran},
                {"Israel Standard Time", TimeZoneId.Israel},
                {"Jordan Standard Time", TimeZoneId.Jordan},
                {"Kamchatka Standard Time", TimeZoneId.Kamchatka},
                {"Korea Standard Time", TimeZoneId.Korea},
                {"Mauritius Standard Time", TimeZoneId.Mauritius},
                {"Mexico Standard Time", TimeZoneId.Mexico},
                {"Mexico Standard Time 2", TimeZoneId.Mexico2},
                {"Mid-Atlantic Standard Time", TimeZoneId.MidAtlantic},
                {"Middle East Standard Time", TimeZoneId.MidEast},
                {"Montevideo Standard Time", TimeZoneId.Montevideo},
                {"Morocco Standard Time", TimeZoneId.Morocco},
                {"Mountain Standard Time", TimeZoneId.Mountain},
                {"Mountain Standard Time (Mexico)", TimeZoneId.MountainMexico},
                {"Myanmar Standard Time", TimeZoneId.Myanmar},
                {"N. Central Asia Standard Time", TimeZoneId.NorthCentralAsia},
                {"Namibia Standard Time", TimeZoneId.Namibia},
                {"Nepal Standard Time", TimeZoneId.Nepal},
                {"New Zealand Standard Time", TimeZoneId.NewZealand},
                {"Newfoundland Standard Time", TimeZoneId.Newfoundland},
                {"North Asia East Standard Time", TimeZoneId.NorthAsiaEast},
                {"North Asia Standard Time", TimeZoneId.NorthAsia},
                {"Pacific SA Standard Time", TimeZoneId.SouthPacific},
                {"Pacific Standard Time", TimeZoneId.Pacific},
                {"Pacific Standard Time (Mexico)", TimeZoneId.PacificMexico},
                {"Pakistan Standard Time", TimeZoneId.Pakistan},
                {"Paraguay Standard Time", TimeZoneId.Paraguay},
                {"Romance Standard Time", TimeZoneId.Romance},
                {"Russian Standard Time", TimeZoneId.Russian},
                {"SA Eastern Standard Time", TimeZoneId.SouthAmericaEastern},
                {"SA Pacific Standard Time", TimeZoneId.SouthAmericaPacific},
                {"SA Western Standard Time", TimeZoneId.SouthAmericaWestern},
                {"Samoa Standard Time", TimeZoneId.MidwayIsland},
                {"SE Asia Standard Time", TimeZoneId.SouthEasternAsia},
                {"Singapore Standard Time", TimeZoneId.Singapore},
                {"South Africa Standard Time", TimeZoneId.SouthAfrica},
                {"Sri Lanka Standard Time", TimeZoneId.SriLanka},
                {"Taipei Standard Time", TimeZoneId.Taipei},
                {"Tasmania Standard Time", TimeZoneId.Tasmania},
                {"Tokyo Standard Time", TimeZoneId.Tokyo},
                {"Tonga Standard Time", TimeZoneId.Tonga},
                {"Ulaanbaatar Standard Time", TimeZoneId.Ulaanbaatar},
                {"US Eastern Standard Time", TimeZoneId.USEastern},
                {"US Mountain Standard Time", TimeZoneId.USMountain},
                {"UTC", TimeZoneId.UTC},
                {"Venezuela Standard Time", TimeZoneId.Venezuela},
                {"Vladivostok Standard Time", TimeZoneId.Vladivostok},
                {"W. Australia Standard Time", TimeZoneId.WestAustralia},
                {"W. Central Africa Standard Time", TimeZoneId.WestCentralAfrica},
                {"W. Europe Standard Time", TimeZoneId.WestEurope},
                {"West Asia Standard Time", TimeZoneId.WestAsia},
                {"West Pacific Standard Time", TimeZoneId.WestPacific},
                {"Yakutsk Standard Time", TimeZoneId.Yakutsk},
                {"Kaliningrad Standard Time", TimeZoneId.Kaliningrad},
                {"Bahia Standard Time", TimeZoneId.Salvador},
                {"Syria Standard Time", TimeZoneId.Damascus}
            };
            return result;
        }
        #endregion
    }
}
