using System.Threading.Tasks;
using FFmpeg.NET.Tests.Fixtures;
using Xunit;

namespace FFmpeg.NET.Tests
{
    public class MetaDataTests : IClassFixture<MediaFileFixture>
    {
        public MetaDataTests(MediaFileFixture fixture)
        {
            _fixture = fixture;
        }

        private readonly MediaFileFixture _fixture;

        [Fact]
        public async Task FFmpeg_Can_Read_Audio_Metadata()
        {
            var ffmpeg = new Engine.FFmpeg();

            var audioFile = _fixture.Audio;
            var metaData = await ffmpeg.GetMetaData(audioFile);

            Assert.NotNull(metaData);

            Assert.NotNull(metaData.AudioData);
            Assert.Equal("mp3", metaData.AudioData.Format);
            Assert.Equal("44100 Hz", metaData.AudioData.SampleRate);
            Assert.Equal("stereo", metaData.AudioData.ChannelOutput);
            Assert.Equal(128, metaData.AudioData.BitRateKbs);

            Assert.Null(metaData.VideoData);
        }

        [Fact]
        public async Task FFmpeg_Can_Read_Video_Metadata()
        {
            var ffmpeg = new Engine.FFmpeg();

            var videoFile = _fixture.Video;
            var metaData = await ffmpeg.GetMetaData(videoFile);

            Assert.NotNull(metaData);
            Assert.NotNull(metaData.VideoData);
            Assert.Equal("h264 (Main) (avc1 / 0x31637661)", metaData.VideoData.Format);
            Assert.Equal("yuv420p,", metaData.VideoData.ColorModel);
            Assert.Equal("1280x720", metaData.VideoData.FrameSize);
            Assert.Equal(1205, metaData.VideoData.BitRateKbs);
            Assert.Equal(25, metaData.VideoData.Fps);

            Assert.NotNull(metaData.AudioData);
            Assert.Equal("aac (LC) (mp4a / 0x6134706D)", metaData.AudioData.Format);
            Assert.Equal("48000 Hz", metaData.AudioData.SampleRate);
            Assert.Equal("5.1", metaData.AudioData.ChannelOutput);
            Assert.Equal(384, metaData.AudioData.BitRateKbs);
        }
    }
}