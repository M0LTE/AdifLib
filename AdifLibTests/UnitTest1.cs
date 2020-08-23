using AdifLib;
using FluentAssertions;
using Xunit;

namespace AdifLibTests
{
    public class UnitTest1
    {
        [Fact]
        public void RecordFromCode()
        {
            var target = new AdifRecord();
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
            AdifRecord.TryParse("<call:5>M0LTE <eor>", out var record, out string error).Should().BeTrue();
            record.Should().NotBeNull();
            error.Should().BeNull();
            record.Call.Should().Be("M0LTE");
            record.Fields["call"].Should().Be("M0LTE");
        }
    }
}
