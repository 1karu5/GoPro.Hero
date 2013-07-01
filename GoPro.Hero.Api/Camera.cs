﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoPro.Hero.Api.Commands;
using GoPro.Hero.Api.Commands.CameraCommands;
using GoPro.Hero.Api.Exceptions;

namespace GoPro.Hero.Api
{
    public class Camera
    {
        public Bacpac Bacpac { get; private set; }
        public CameraInformation Information { get; private set; }
        public CameraExtendedSettings ExtendedSettings { get; private set; }
        public CameraSettings Settings { get; private set; }

        public void UpdateInformation()
        {
            var request = this.CreateCommand<CommandCameraInformation>();
            var response = request.Send();

            var stream = response.GetResponseStream();
            this.Information.Update(stream);
        }

        public void UpdateSettings()
        {
            var request = this.CreateCommand<CommandCameraSettings>();
            var response = request.Send();

            var stream = response.GetResponseStream();
            this.Settings.Update(stream);
        }

        public void UpdateExtendedSettings()
        {
            var request = this.CreateCommand<CommandCameraExtendedSettings>();
            var response = request.Send();

            var stream = response.GetResponseStream();
            this.ExtendedSettings.Update(stream);
        }

        private T CreateCommand<T>(string parameter = null) where T : CommandRequest
        {
            var request = CommandRequest.Create<T>(this.Bacpac.Address, passPhrase: this.Bacpac.Password, parameter: parameter);
            return request;
        }

        private Camera(Bacpac bacpac)
        {
            this.Bacpac = bacpac;
            this.Information = new CameraInformation();
            this.ExtendedSettings = new CameraExtendedSettings();
            this.Settings = new CameraSettings();

            this.UpdateInformation();
            this.UpdateSettings();
            this.UpdateExtendedSettings();
        }

        public static Camera Create(Bacpac bacpac)
        {
            var camera = new Camera(bacpac);
            return camera;
        }
    }
}
