﻿using System;
using System.IO;
using Camera.Model;
using MonoTouch.FacebookConnect;
using MonoTouch.Foundation;
using TinyMessenger;

namespace Camera.Helpers
{
    public class StateManager : IStateManager
    {


        public StateManager()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "usnapus.sqlite");
            lock (_dbLock)
            {
                if (Db == null)
                {
                    Db = new SQLiteConnection(dbPath);
                }
                DoMigrations(Db);
            }
        }


        DeviceRegistration _currentDeviceRegistration;

        public DeviceRegistration CurrentDeviceRegistration
        {
            set
            {
                lock (_dbLock)
                {

                    if (_currentDeviceRegistration != null)
                    {
                        Db.Delete(_currentDeviceRegistration);
                    }
                    if (value != null)
                    {
                        Db.Insert(value);
                    }
                    _currentDeviceRegistration = value;

                }
            }
            get { return _currentDeviceRegistration; }
        }

        void RegisterDevice(string deviceName)
        {
            var deviceRegistrationDetails = StateManager.Db.Find<DeviceRegistration > (reg => true);
            if (deviceRegistrationDetails == null)
            {

                CurrentDeviceRegistration = Server.RegisterDevice(new DeviceRegistration {
                    Guid = Guid.NewGuid().ToString("N"),
                    Name = deviceName
                });
            }
            else
                _currentDeviceRegistration = deviceRegistrationDetails;
        }

        void DoMigrations(SQLiteConnection db)
        {
            db.CreateTable<DeviceRegistration>();
            //db.CreateTable<CurrentEvent>();
            //db.DeleteAll<CurrentEvent>();
            //db.CreateTable<Photo>();
        }

        static IStateManager _stateManager;

        public static IStateManager Current
        {
            get { return _stateManager ?? (_stateManager = new StateManager()); }
            set { _stateManager = value; }
        }

        IServer _server;
        readonly object _dbLock = new object();
        internal static SQLiteConnection Db;
        static ITinyMessengerHub _messageHub;
        ILogger _logger;

        public ITinyMessengerHub MessageHub
        {
            get { return _messageHub ?? (_messageHub = new TinyMessengerHub()); }
            set { _messageHub = value; }
        }

        public IServer Server
        {
            get { return _server ?? (_server = new Server(MessageHub, Logger)); }
            set { _server = value; }
        }

        protected ILogger Logger
        {
            get { return _logger ?? (_logger = new RaygunLogger()); }
            set { _logger = value; }
        }



        public string DeviceName
        {
            set
            {
                RegisterDevice(value);
            }
        }

        public User CurrentUser { get; set; }


        public void UpdateDeviceRegistration(string name, string email, string facebookId)
        {

            if (_currentDeviceRegistration != null)
            {
                lock (_dbLock)
                {
                    _currentDeviceRegistration.Name = name;
                    _currentDeviceRegistration.Email = email;
                    _currentDeviceRegistration.FacebookId = facebookId;
                    var savedDevice = Server.RegisterDevice(_currentDeviceRegistration);
                    if (savedDevice != null)
                    {
                        _currentDeviceRegistration = savedDevice;
                        Db.Insert(_currentDeviceRegistration);
                    }
                }
            }
            else
            {
                CurrentDeviceRegistration = Server.RegisterDevice(new DeviceRegistration {
                    Email = email,
                    Name = name,
                    FacebookId = facebookId,
                    Guid = Guid.NewGuid().ToString("N")
                });
            }
            Logger.Trace("exit");
        }

        public void InitiateFacebookLogin()
        {
            FacebookSession.Current.InitiateLogin();
        }

      


        public void Dispose()
        {
            lock (_dbLock)
            {
                Logger.Trace("enter");
                if (Db != null)
                {
                    Db.Dispose();
                    Db = null;
                }
            }
            Logger.Trace("exit");
        }
    }

    
}