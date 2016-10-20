namespace EasyNetQ.Management.Client.Tests.Serialization
{
    using Client.Model;
    using Client.Serialization;
    using Newtonsoft.Json;
    using Xunit;

    public class HaParamsConverterTests
    {
        [Fact]
        public void Should_only_handle_HaParams()
        {
            Assert.True(new HaParamsConverter().CanConvert(typeof(HaParams)));
            Assert.False(new HaParamsConverter().CanConvert(typeof(HaMode)));
        }

        [Fact]
        public void Should_deserialize_exactly_count()
        {
            var deserializedObject = JsonConvert.DeserializeObject<HaParams>("5", new HaParamsConverter());
            Assert.NotNull(deserializedObject);
            Assert.Equal(HaMode.Exactly, deserializedObject.AssociatedHaMode);
            Assert.Equal(5L, deserializedObject.ExactlyCount);
        }

        [Fact]
        public void Should_deserialize_nodes_list()
        {
            var deserializedObject = JsonConvert.DeserializeObject<HaParams>("[\"a\", \"b\"]", new HaParamsConverter());
            Assert.NotNull(deserializedObject);
            Assert.Equal(HaMode.Nodes, deserializedObject.AssociatedHaMode);
            Assert.NotNull(deserializedObject.Nodes);
            Assert.Equal(2, deserializedObject.Nodes.Length);
            Assert.Contains("a", deserializedObject.Nodes);
            Assert.Contains("b", deserializedObject.Nodes);
        }

        [Fact]
        public void Should_not_be_able_to_deserialize_non_string_list()
        {
            Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<HaParams>("[1,2]", new HaParamsConverter()));
        }

        [Fact]
        public void Should_be_able_to_serialize_count()
        {
            Assert.Equal("2", JsonConvert.SerializeObject(new HaParams{AssociatedHaMode = HaMode.Exactly, ExactlyCount = 2}, new HaParamsConverter()));
        }

        [Fact]
        public void Should_be_able_to_serialize_list()
        {
            Assert.Equal(JsonConvert.SerializeObject(new[] { "a", "b" }), JsonConvert.SerializeObject(new HaParams { AssociatedHaMode = HaMode.Nodes, Nodes = new[] { "a", "b" } }, new HaParamsConverter()));
        }

        [Fact]
        public void Should_not_be_able_to_serialize_null_nodes_list()
        {
            Assert.Throws<JsonSerializationException>(() => JsonConvert.SerializeObject(new HaParams { AssociatedHaMode = HaMode.Nodes }, new HaParamsConverter()));
        }

        [Fact]
        public void Should_not_be_able_to_serialize_all_type()
        {
            Assert.Throws<JsonSerializationException>(() => JsonConvert.SerializeObject(new HaParams { AssociatedHaMode = HaMode.All, Nodes = new[] { "a", "b" } }, new HaParamsConverter()));
        }
    }
}
