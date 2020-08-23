using FluentAssertions;
using M0LTE.AdifLib;
using System;
using System.Globalization;
using Xunit;

namespace AdifLibTests
{
    public class UnitTest1
    {
        [Fact]
        public void RecordFromCode()
        {
            var target = new AdifContactRecord();
            string call = "M0LTE";
            target.Call = call;
            target.Fields.Should().ContainKey("call");
            target.Fields["call"].Should().Be(call);
            target.Call.Should().Be(call);
            target.ToString().Should().Be("<call:5>M0LTE <eor>");
        }

        [Fact]
        public void ObjectFromRecord()
        {
            AdifContactRecord.TryParse("<call:5>M0LTE <eor>", out var record, out string error).Should().BeTrue();
            record.Should().NotBeNull();
            error.Should().BeNull();
            record.Call.Should().Be("M0LTE");
            record.Fields["call"].Should().Be("M0LTE");
        }

        [Fact]
        public void FileObjectFromString()
        {
            string adif = @"
<adif_ver:5>3.1.0
<programid:6>WSJT-X
<EOH>
<call:6>DL8RBL <gridsquare:4>JN68 <mode:3>FT8 <rst_sent:3>+01 <rst_rcvd:3>-01 <qso_date:8>20200823 <time_on:6>153730 <qso_date_off:8>20200823 <time_off:6>153830 <band:3>10m <freq:9>28.074712 <station_callsign:5>M0LTE <my_gridsquare:6>IO91LK <tx_pwr:3>30W <EOR>
<call:6>2E1EPQ <EOR>";

            AdifFile.TryParse(adif, out var adifFile).Should().BeTrue();
            adifFile.Header.Should().NotBeNull();
            adifFile.Header.AdifVersion.Should().Be("3.1.0");
            adifFile.Header.ProgramId.Should().Be("WSJT-X");
            adifFile.Records.Should().NotBeNull();
            adifFile.Records.Should().HaveCount(2);
            adifFile.Records[0].Call.Should().Be("DL8RBL");
            adifFile.Records[0].GridSquare.Should().Be("JN68");
            adifFile.Records[0].Mode.Should().Be("FT8");
            adifFile.Records[0].RstSent.Should().Be("+01");
            adifFile.Records[0].RstReceived.Should().Be("-01");
            adifFile.Records[0].QsoStart.Should().Be(DateTime.Parse("2020-08-23T15:37:30Z", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal));
            adifFile.Records[0].QsoEnd.Should().Be(DateTime.Parse("2020-08-23T15:38:30Z", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal));
            adifFile.Records[0].Band.Should().Be("10m");
            adifFile.Records[0].FreqMHz.Should().Be("28.074712");
            adifFile.Records[0].StationCallsign.Should().Be("M0LTE");
            adifFile.Records[0].MyGridSquare.Should().Be("IO91LK");
            adifFile.Records[0].TxPower.Should().Be("30W");
            adifFile.Records[1].Call.Should().Be("2E1EPQ");
        }
    }
}
