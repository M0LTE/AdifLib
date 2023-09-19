using System;

namespace M0LTE.AdifLib
{
    public class AdifContactRecord :AdifRecord
    {
        protected override string EndMarker => "<eor>";

        public DateTime QsoStart
        {
            get => GetQsoDateTime("qso_date", "time_on");
            set => SetQsoDateTime("qso_date", "time_on", value);
        }

        public DateTime QsoEnd
        {
            get
            {
                var result = GetQsoDateTime("qso_date_off", "time_off");
                return result != default ? result : GetQsoDateTime("qso_date", "time_off");
            }

            set => SetQsoDateTime("qso_date_off", "time_off", value);
        }

        private void SetQsoDateTime(string dateField, string timeField, DateTime value)
        {
            Fields[dateField] = value.ToUniversalTime().ToString("yyyyMMdd");
            Fields[timeField] = value.ToUniversalTime().ToString("HHmmss");
        }

        private DateTime GetQsoDateTime(string dateField, string timeField)
        {
            if (!Fields.TryGetValue(dateField, out string date)) return default;
            if (!Fields.TryGetValue(timeField, out string time)) return default;
            if (date.Length != 8) return default;
            if (time.Length != 6 && time.Length != 4) return default;

            if (!int.TryParse(date.Substring(0, 4), out int year) || year < 1900 || year > 2100) return default;
            if (!int.TryParse(date.Substring(4, 2), out int month) || month > 12 || month < 0) return default;
            if (!int.TryParse(date.Substring(6, 2), out int day) || day < 1 || day > 31) return default;
            if (!int.TryParse(time.Substring(0, 2), out int hour) || hour >= 24 || hour < 0) return default;
            if (!int.TryParse(time.Substring(2, 2), out int min) || min >= 60 || min < 0) return default;
            int sec = 0;
            if (time.Length == 6)
            {
                if (!int.TryParse(time.Substring(4, 2), out sec) || sec >= 60 || sec < 0) return default;
            }
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

        public string SubMode
        {
            get => Fields.TryGetValue("submode", out string value) ? value : null;
            set => SetField("submode", value);
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

        public string Country
        {
            get => Fields.TryGetValue("country", out string value) ? value : null;
            set => SetField("country", value);
        }

        public string FreqMHz
        {
            get => Fields.TryGetValue("freq", out string value) ? value : null;
            set => SetField("freq", value);
        }

        public string FreqRxMHz
        {
            get => Fields.TryGetValue("freq_rx", out string value) ? value : null;
            set => SetField("freq_rx", value);
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
        
        public string Operator
        {
            get => Fields.TryGetValue("operator", out string value) ? value : null;
            set => SetField("operator", value);
        }

        public int? CQZone
        {
            get => Fields.TryGetValue("cqz", out string value) ? int.Parse(value) : (int?)null;
            set => SetField("cqz", value == null ? null : value.ToString());
        }

        public string TransmittedSerial
        {
            get => Fields.TryGetValue("stx", out string value) ? value : null;
            set => SetField("stx", value);
        }
    }
}