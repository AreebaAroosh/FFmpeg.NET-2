﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FFmpeg.NET.Services;
using FFmpeg.NET.Tests.Fixtures;
using Xunit;

namespace FFmpeg.NET.Tests
{
    public class PlaylistCreatorTests : IClassFixture<MediaFileFixture>
    {
        public PlaylistCreatorTests(MediaFileFixture fixture)
        {
            _fixture = fixture;
        }

        private readonly MediaFileFixture _fixture;

        [Fact]
        public async Task M3uPlaylistCreator_Creates_Valid_m3u8_Content()
        {
            var ffmpeg = new Engine.FFmpeg();
            var meta1 = await ffmpeg.GetMetaData(_fixture.Video);
            var meta2 = await ffmpeg.GetMetaData(_fixture.Audio);

            Assert.NotNull(meta1);
            Assert.NotNull(meta2);

            var m3u8 = new M3uPlaylistCreator().Create(new Dictionary<FileInfo,MetaData>{{_fixture.VideoFile, meta1}, { _fixture.AudioFile, meta2 } });

            Assert.NotNull(m3u8);

            var lines = m3u8.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            Assert.True(lines.Length == 5);

            Assert.Equal("#EXTM3U", lines[0]);
            Assert.Equal("#EXTINF:5,SampleVideo_1280x720_1mb.mp4", lines[1]);
            Assert.Equal($"file:///{_fixture.VideoFile.FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)}", lines[2]);
            Assert.Equal("#EXTINF:27,SampleAudio_0.4mb.mp3", lines[3]);
            Assert.Equal($"file:///{_fixture.AudioFile.FullName.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)}", lines[4]);
        }

        [Fact]
        public async Task XspfPlaylistCreator_Creates_Valid_Xml()
        {
            var ffmpeg = new Engine.FFmpeg();
            var meta1 = await ffmpeg.GetMetaData(_fixture.Video);
            var meta2 = await ffmpeg.GetMetaData(_fixture.Audio);

            Assert.NotNull(meta1);
            Assert.NotNull(meta2);

            var xml = new XspfPlaylistCreator().Create(new Dictionary<FileInfo, MetaData> { { _fixture.VideoFile, meta1 }, { _fixture.AudioFile, meta2 } });

            Assert.NotNull(xml);
            Assert.NotEmpty(xml);

            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("FFmpeg.NET.Tests.Resources.test.xspf"))
            using (var sr = new StreamReader(resource))
            {
                var xspf = sr.ReadToEnd();

                Assert.NotNull(xspf);

                var assemblyPath = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
                var file1 = Path.GetRelativePath(assemblyPath, _fixture.VideoFile.FullName);
                var file2 = Path.GetRelativePath(assemblyPath, _fixture.AudioFile.FullName);

                Assert.NotNull(file1);
                Assert.NotNull(file2);
                Assert.Contains($"file:///{file1.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)}", xspf);
                Assert.Contains($"file:///{file2.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)}", xspf);
            }
        }
    }
}