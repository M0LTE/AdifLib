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

        [Fact]
        public void SampleFile1()
        {
            const string adif = @"Cloudlog ADIF export
<ADIF_VER:5>3.1.2
<PROGRAMID:8>Cloudlog
<PROGRAMVERSION:11>Version 1.7
<EOH>

<call:5>GX3CO <gridsquare:6>JO01KV <mode:3>FT8 <rst_sent:3>+11 <rst_rcvd:3>+12 <qso_date:8>20220409 <time_on:6>160030 <qso_date_off:8>20220409 <time_off:6>160130 <band:3>40m <freq:8>7.075685 <station_callsign:5>M0LTE <my_gridsquare:6>IO91LK <tx_pwr:4>200W <operator:5>M0LTE <eor>
<call:5>G6YWL <gridsquare:6>IO82XN <mode:3>FT8 <rst_sent:3>+18 <rst_rcvd:3>+07 <qso_date:8>20220409 <time_on:6>160630 <qso_date_off:8>20220409 <time_off:6>160730 <band:3>40m <freq:8>7.075685 <station_callsign:5>M0LTE <my_gridsquare:6>IO91LK <tx_pwr:4>200W <operator:5>M0LTE <eor>
<call:5>G0FOG <gridsquare:6>IO92NW <mode:3>FT8 <rst_sent:3>+12 <rst_rcvd:3>+19 <qso_date:8>20220409 <time_on:6>160815 <qso_date_off:8>20220409 <time_off:6>160915 <band:3>40m <freq:8>7.075685 <station_callsign:5>M0LTE <my_gridsquare:6>IO91LK <tx_pwr:4>200W <operator:5>M0LTE <eor>
<call:5>ON3CH <gridsquare:6>JO20WO <mode:3>FT8 <rst_sent:3>-17 <rst_rcvd:3>-08 <qso_date:8>20220409 <time_on:6>161000 <qso_date_off:8>20220409 <time_off:6>161100 <band:3>40m <freq:8>7.075685 <station_callsign:5>M0LTE <my_gridsquare:6>IO91LK <tx_pwr:4>200W <operator:5>M0LTE <eor>
<call:5>F4FSY <gridsquare:6>JN25KM <mode:3>FT8 <rst_sent:3>+12 <rst_rcvd:3>-02 <qso_date:8>20220409 <time_on:6>161200 <qso_date_off:8>20220409 <time_off:6>161300 <band:3>40m <freq:8>7.075685 <station_callsign:5>M0LTE <my_gridsquare:6>IO91LK <tx_pwr:4>200W <operator:5>M0LTE <eor>
<call:5>M6HTF <gridsquare:6>IO91SR <mode:3>FT8 <rst_sent:3>+08 <rst_rcvd:3>+23 <qso_date:8>20220409 <time_on:6>161330 <qso_date_off:8>20220409 <time_off:6>161430 <band:3>40m <freq:8>7.075685 <station_callsign:5>M0LTE <my_gridsquare:6>IO91LK <tx_pwr:4>200W <operator:5>M0LTE <eor>
<call:5>GX3CO <gridsquare:6>JO01KV <mode:3>FT8 <rst_sent:3>+07 <rst_rcvd:3>+11 <qso_date:8>20220409 <time_on:6>161652 <qso_date_off:8>20220409 <time_off:6>161730 <band:3>40m <freq:8>7.075685 <station_callsign:5>M0LTE <my_gridsquare:6>IO91LK <tx_pwr:4>200W <operator:5>M0LTE <eor>";

            AdifFile.TryParse(adif, out var file).Should().BeTrue();

            file.Records[6].Call.Should().Be("GX3CO");
            file.Records[6].GridSquare.Should().Be("JO01KV");
            file.Records[6].Mode.Should().Be("FT8");
            file.Records[6].RstSent.Should().Be("+07");
            file.Records[6].RstReceived.Should().Be("+11");
            file.Records[6].QsoStart.Should().Be(new DateTime(2022, 4, 9, 16, 16, 52));
            file.Records[6].Band.Should().Be("40m");
            file.Records[6].FreqMHz.Should().Be("7.075685");
            file.Records[6].StationCallsign.Should().Be("M0LTE");
            file.Records[6].MyGridSquare.Should().Be("IO91LK");
            file.Records[6].TxPower.Should().Be("200W");
            file.Records[6].Fields["operator"].Should().Be("M0LTE");
        }

        [Fact]
        public void N1mmAdifExport()
        {
            const string adif = @"ADIF Export from N1MMLogger.net - Version 1.0.9700.0
Built: 25/10/2022 13:21:24
M0LTE logs generated @ 2022-10-30 22:12:51Z
Contest Name: CQWWSSB - 2022-10-29
<EOH>
 <CALL:4>UA4M <QSO_DATE:8>20221029 <TIME_ON:6>181411 <TIME_OFF:6>181415 <BAND:3>40M <STATION_CALLSIGN:5>M0LTE <FREQ:7>7.10400 <CONTEST_ID:9>CQ-WW-SSB <FREQ_RX:7>7.10500 <MODE:3>SSB <RST_RCVD:2>59 <RST_SENT:2>58 <OPERATOR:5>M0LTE <CQZ:2>16 <STX:1>1 <APP_N1MM_POINTS:1>1 <APP_N1MM_RADIO_NR:1>1 <APP_N1MM_CONTINENT:2>EU <APP_N1MM_RUN1RUN2:1>1 <APP_N1MM_RADIOINTERFACED:1>1 <APP_N1MM_ISORIGINAL:4>True <APP_N1MM_NETBIOSNAME:8>STUDY-PC <APP_N1MM_ISRUNQSO:1>0 <PFX:3>UA4 <APP_N1MM_MULT1:1>1 <APP_N1MM_MULT2:1>1 <APP_N1MM_MULT3:1>0 <APP_N1MM_ID:32>9e3c709fa5084f82bae05f1ec48b9cd3 <APP_N1MM_CLAIMEDQSO:1>1 <EOR>
 <CALL:5>PA6DX <QSO_DATE:8>20221029 <TIME_ON:6>181433 <TIME_OFF:6>181433 <BAND:3>40M <STATION_CALLSIGN:5>M0LTE <FREQ:7>7.10950 <CONTEST_ID:9>CQ-WW-SSB <FREQ_RX:7>7.10950 <MODE:3>SSB <RST_RCVD:2>59 <RST_SENT:2>59 <OPERATOR:5>M0LTE <CQZ:2>14 <STX:1>2 <APP_N1MM_POINTS:1>1 <APP_N1MM_RADIO_NR:1>1 <APP_N1MM_CONTINENT:2>EU <APP_N1MM_RUN1RUN2:1>1 <APP_N1MM_RADIOINTERFACED:1>1 <APP_N1MM_ISORIGINAL:4>True <APP_N1MM_NETBIOSNAME:8>STUDY-PC <APP_N1MM_ISRUNQSO:1>0 <PFX:3>PA6 <APP_N1MM_MULT1:1>1 <APP_N1MM_MULT2:1>1 <APP_N1MM_MULT3:1>0 <APP_N1MM_ID:32>9e2f25b583df420e844d31c32369ff78 <APP_N1MM_CLAIMEDQSO:1>1 <EOR>";

            AdifFile.TryParse(adif, out var file).Should().BeTrue();

            file.Records.Count.Should().Be(2);

            var r1 = file.Records[0];
            r1.Call.Should().Be("UA4M");
            r1.QsoStart.Should().Be(new DateTime(2022, 10, 29, 18, 14, 11));
            r1.QsoEnd.Should().Be(new DateTime(2022, 10, 29, 18, 14, 15));
            r1.Band.Should().Be("40M");
            r1.StationCallsign.Should().Be("M0LTE");
            r1.FreqMHz.Should().Be("7.10400");
            r1.Fields["contest_id"].Should().Be("CQ-WW-SSB");
            r1.FreqRxMHz.Should().Be("7.10500");
            r1.Mode.Should().Be("SSB");
            r1.RstReceived.Should().Be("59");
            r1.RstSent.Should().Be("58");
            r1.Operator.Should().Be("M0LTE");
            r1.CQZone.Should().Be(16);
            r1.TransmittedSerial.Should().Be("1");
        }
    }
}
