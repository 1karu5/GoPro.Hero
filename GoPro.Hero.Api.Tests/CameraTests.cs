﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GoPro.Hero.Api.Commands;
using GoPro.Hero.Api.Commands.CameraCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoPro.Hero.Api.Tests
{
    [TestClass]
    public class CameraTests
    {
        private Camera GetCamera()
        {
            var bacpac = Bacpac.Create(ExpectedParameters.IP_ADDRESS);
            var camera = Camera.Create<Camera>(bacpac);

            return camera;
        }

        private void ChangeSelection<T, S>(Camera camera, S select, Func<ICamera, S> valueRetriever) where T : CommandMultiChoice<S,ICamera>
        {
            var command = camera.PrepareCommand<T>();
            command.Selection = select;

            var res = valueRetriever(camera.Command(command));
            Assert.AreEqual(select, res);
        }

        public void CheckMultiChoiceCommand<T, S>(Func<ICamera, S> valueRetriever)where T:CommandMultiChoice<S,ICamera>
        {
            var camera = GetCamera();
            var init = valueRetriever(camera);

            var selection=Enum.GetValues(typeof(S));

            foreach(var selected in selection)
            {
                var value=(S)Convert.ChangeType(selected,typeof(S));
                ChangeSelection<T,S>(camera,value,valueRetriever);
            }

            ChangeSelection<T,S>(camera,init,valueRetriever);
        }

        private void CheckBooleanCommand<T>(Func<ICamera, bool> valueRetriever) where T : CommandBoolean<ICamera>
        {
            var camera = GetCamera();
            var init = valueRetriever(camera);

            var command = camera.PrepareCommand<T>();

            command.State = true;
            var res = valueRetriever(camera.Command(command));
            Assert.AreEqual(true, res);

            command.State = false;
            res = valueRetriever(camera.Command(command));
            Assert.AreEqual(false, res);

            command.State = init;
            camera.Command(command);
        }

        [TestInitialize]
        public void InitializeCamera()
        {
            var camera = GetCamera();

            camera.Power(true);
            Thread.Sleep(2000);
            var res = camera.BacpacStatus.CameraPower;
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void CheckGetName()
        {
            var camera = this.GetCamera();

            var name = camera.GetName();
            Assert.AreEqual(ExpectedParameters.REAL_NAME, name);
        }

        [TestMethod]
        public void CheckSetName()
        {
            var camera = GetCamera();
            var init = camera.GetName();

            camera.SetName(ExpectedParameters.TEMP_NAME);
            var tempName=camera.GetName();
            Assert.AreEqual(ExpectedParameters.TEMP_NAME, tempName);
        }

        [TestMethod]
        public void CheckVideoResolution()
        {
            CheckMultiChoiceCommand<CommandCameraVideoResolution, VideoResolution>((c) => c.ExtendedSettings.VideoResolution);
        }

        [TestMethod]
        public void CheckOrientation()
        {
            CheckMultiChoiceCommand<CommandCameraOrientation, Orientation>((c) => c.ExtendedSettings.Orientation);
        }

        [TestMethod]
        public void CheckTimeLapse()
        {
            CheckMultiChoiceCommand<CommandCameraTimeLapse, TimeLapse>((c) => c.ExtendedSettings.TimeLapse);
        }

        [TestMethod]
        public void CheckBeepSound()
        {
            CheckMultiChoiceCommand<CommandCameraBeepSound, BeepSound>((c) => c.ExtendedSettings.BeepSound);
        }

        [TestMethod]
        public void CheckProtune()
        {
            CheckBooleanCommand<CommandCameraProtune>((c) => c.ExtendedSettings.ProTune);
        }

        [TestMethod]
        public void CheckPhotoResolution()
        {
            CheckMultiChoiceCommand<CommandCameraPhotoResolution, PhotoResolution>((c) => c.ExtendedSettings.PhotoResolution);
        }

        [TestMethod]
        public void CheckVideoStandard()
        {
            CheckMultiChoiceCommand<CommandCameraVideoStandard, VideoStandard>((c) => c.ExtendedSettings.VideoStandard);
        }

        [TestMethod]
        public void CheckModes()
        {
            CheckMultiChoiceCommand<CommandCameraMode, Mode>((c) => c.ExtendedSettings.Mode);
        }

        [TestMethod]
        public void LocateCamera()
        {
            CheckBooleanCommand<CommandCameraLocate>((c) => c.ExtendedSettings.LocateCamera);
        }

        [TestMethod]
        public void CheckPreview()
        {
            var camera = GetCamera();
            var available = camera.ExtendedSettings.PreviewAvailable;
            Assert.AreEqual(true, available);

            CheckBooleanCommand<CommandCameraPreview>((c) => c.ExtendedSettings.PreviewActive);
        }

        [TestMethod]
        public void CheckLedBlinks()
        {
            CheckMultiChoiceCommand<CommandCameraLedBlink, LedBlink>((c) => c.ExtendedSettings.LedBlink);
        }

        [TestMethod]
        public void CheckFieldOfView()
        {
            CheckMultiChoiceCommand<CommandCameraFieldOfView, FieldOfView>((c) => c.ExtendedSettings.FieldOfView);
        }

        [TestMethod]
        public void CheckSpotMeter()
        {
            CheckBooleanCommand<CommandCameraSpotMeter>((c) => c.ExtendedSettings.SpotMeter);
        }

        [TestMethod]
        public void CheckOnDefaultMode()
        {
            CheckMultiChoiceCommand<CommandCameraDefaultMode, Mode>((c) => c.ExtendedSettings.OnDefault);
        }

        [TestMethod]
        public void CheckDeleteAllOnSdCard()
        {
            var camera = this.GetCamera();

            CommandCameraDeleteAllFilesOnSd command;
            var photo = camera.PrepareCommand<CommandCameraDeleteAllFilesOnSd>(out command).Command(command).ExtendedSettings.PhotosCount;
            var video = camera.ExtendedSettings.VideosCount;

            Assert.IsTrue(photo == 0 && video == 0);
        }

        [TestMethod]
        public void CheckDeleteLastOnSdCard()
        {
            var camera = this.GetCamera();
            var initPhoto = camera.ExtendedSettings.PhotosCount;
            var initVideo = camera.ExtendedSettings.VideosCount;

            CommandCameraDeleteLastFileOnSd command;
            var photo = camera.PrepareCommand<CommandCameraDeleteLastFileOnSd>(out command).Command(command).ExtendedSettings.PhotosCount;
            var video = camera.ExtendedSettings.VideosCount;

            Assert.IsTrue(photo < initPhoto || video < initVideo);
        }

        [TestMethod]
        public void CheckCameraInformation()
        {
            var camera = this.GetCamera();
            var info=camera.Information;

            Assert.AreEqual(ExpectedParameters.NAME, info.Name);
        }

        [TestMethod]
        public void CheckCameraSettings()
        {
            var camera = this.GetCamera();
            var settings = camera.Settings;

            Assert.AreEqual(ExpectedParameters.DEFAULT_VIDEO, settings.VideoStandard);
        }

        [TestMethod]
        public void CheckCameraExtendedSettings()
        {
            var camera = this.GetCamera();
            var extendedSettings = camera.ExtendedSettings;

            Assert.AreEqual(ExpectedParameters.DEFAULT_VIDEO, extendedSettings.VideoStandard);
        }

        [TestMethod]
        public void CheckWhiteBalance()
        {
            var camera = GetCamera();
            var protune = camera.ExtendedSettings.ProTune;
            Assert.AreEqual(protune, true);

            CheckMultiChoiceCommand<CommandCameraWhiteBalance, WhiteBalance>((c) => c.ExtendedSettings.WhiteBalance);
        }

        [TestMethod]
        public void CheckLoopingVideo()
        {
            CheckMultiChoiceCommand<CommandCameraLoopingVideo, LoopingVideo>((c) => c.ExtendedSettings.LoopingVideoMode);
        }


        [TestMethod]
        public void CheckFrameRate()
        {
            CheckMultiChoiceCommand<CommandCameraFrameRate, FrameRate>((c) => c.ExtendedSettings.FrameRate);
        }

        [TestMethod]
        public void CheckBurstRate()
        {
            CheckMultiChoiceCommand<CommandCameraBurstRate, BurstRate>((c) => c.ExtendedSettings.BurstRate);
        }

        [TestMethod]
        public void CheckContinuousShot()
        {
            CheckMultiChoiceCommand<CommandCameraContinuousShot, ContinuousShot>((c) => c.ExtendedSettings.ContinuousShot);
        }
    }
}
