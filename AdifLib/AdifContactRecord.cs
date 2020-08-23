using System;

namespace M0LTE.AdifLib
{
    public class AdifContactRecord :AdifRecord
    {
        public DateTime QsoStart
        {
            get => GetQsoDateTime("qso_date", "time_on");
            set => SetQsoDateTime("qso_date", "time_on", value);
        }

        public DateTime QsoEnd
        {
            get => GetQsoDateTime("qso_date_off", "time_off");
            set => SetQsoDateTime("qso_date_off", "time_off", value);
        }

        private void SetQsoDateTime(string dateField, string timeField, DateTime value)
        {
            Fields[dateField] = value.ToString("yyyyMMdd");
            Fields[timeField] = value.ToString("HHmmss");
        }

        private DateTime GetQsoDateTime(string dateField, string timeField)
        {
            if (!Fields.TryGetValue(dateField, out string date)) return default;
            if (!Fields.TryGetValue(timeField, out string time)) return default;
            if (date.Length != 8) return default;
            if (time.Length != 6) return default;

            if (!int.TryParse(date.Substring(0, 4), out int year)) return default;
            if (!int.TryParse(date.Substring(4, 2), out int month)) return default;
            if (!int.TryParse(date.Substring(6, 2), out int day)) return default;
            if (!int.TryParse(time.Substring(0, 2), out int hour)) return default;
            if (!int.TryParse(time.Substring(2, 2), out int min)) return default;
            if (!int.TryParse(time.Substring(4, 2), out int sec)) return default;

            return new DateTime(year, month, day, hour, min, sec, DateTimeKind.Utc);
        }

        public string Call
        {
            get => Fields.TryGetValue("call", out string value) ? value : null;
            set => SetField("call", value);
        }

        public string GridSquare
        {
            get => Fields.TryGetValue("gridsquare", out string value) ? value : null;
            set => SetField("gridsquare", value);
        }

        public string Mode
        {
            get => Fields.TryGetValue("mode", out string value) ? value : null;
            set => SetField("mode", value);
        }

        public string RstSent
        {
            get => Fields.TryGetValue("rst_sent", out string value) ? value : null;
            set => SetField("rst_sent", value);
        }

        public string RstReceived
        {
            get => Fields.TryGetValue("rst_rcvd", out string value) ? value : null;
            set => SetField("rst_rcvd", value);
        }

        public string Band
        {
            get => Fields.TryGetValue("band", out string value) ? value : null;
            set => SetField("band", value);
        }

        public string FreqMHz
        {
            get => Fields.TryGetValue("freq", out string value) ? value : null;
            set => SetField("freq", value);
        }

        public string StationCallsign
        {
            get => Fields.TryGetValue("station_callsign", out string value) ? value : null;
            set => SetField("station_callsign", value);
        }

        public string MyGridSquare
        {
            get => Fields.TryGetValue("my_gridsquare", out string value) ? value : null;
            set => SetField("my_gridsquare", value);
        }

        public string TxPower
        {
            get => Fields.TryGetValue("tx_pwr", out string value) ? value : null;
            set => SetField("tx_pwr", value);
        }

        public static bool TryParse(string record, out AdifContactRecord adifContactRecord, out string error)
        {
            if (AdifRecord.TryParse(record, out var adifRecord, out error))
            {
                adifContactRecord = new AdifContactRecord();

                foreach (var kvp in adifRecord.Fields)
                {
                    adifContactRecord.Fields.Add(kvp.Key, kvp.Value);
                }

                error = null;
                return true;
            }

            adifContactRecord = null;
            return false;
        }
    }
}