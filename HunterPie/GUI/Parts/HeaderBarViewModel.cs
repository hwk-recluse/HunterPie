﻿using HunterPie.Core.Architecture;
using HunterPie.Core.Client;
using HunterPie.Integrations.Poogie.Common.Models;
using HunterPie.Integrations.Poogie.Supporter;
using HunterPie.Integrations.Poogie.Supporter.Models;
using System;
using System.Reflection;
using System.Security.Principal;
using System.Windows;

namespace HunterPie.GUI.Parts;

public class HeaderBarViewModel : Bindable
{
    private readonly PoogieSupporterConnector _supporterConnector = new();
    private bool _isSupporter;
    private bool _isFetchingSupporter;
    private bool _isNotificationsToggled;

    public string Version
    {
        get
        {
            Assembly self = typeof(App).Assembly;
            AssemblyName name = self.GetName();
            Version ver = name.Version;

            return $"v{ver}";
        }
    }

    public bool IsSupporter { get => _isSupporter; set => SetValue(ref _isSupporter, value); }
    public bool IsFetchingSupporter { get => _isFetchingSupporter; set => SetValue(ref _isFetchingSupporter, value); }
    public bool IsNotificationsToggled { get => _isNotificationsToggled; set => SetValue(ref _isNotificationsToggled, value); }

    public Visibility IsRunningAsAdmin => GetAdminState()
                                          ? Visibility.Visible
                                          : Visibility.Collapsed;

    public void MinimizeApplication()
    {
        Application.Current.MainWindow.WindowState = WindowState.Minimized;

        if (ClientConfig.Config.Client.MinimizeToSystemTray)
            Application.Current.MainWindow.Hide();
    }

    public async void FetchSupporterStatus()
    {
        IsFetchingSupporter = true;

        PoogieResult<SupporterValidationResponse> res = await _supporterConnector.IsValidSupporter();

        IsSupporter = res?.Response?.IsValid ?? false;

        IsFetchingSupporter = false;
    }

    public void CloseApplication() => Application.Current.MainWindow?.Close();

    public void DragApplication() => Application.Current.MainWindow?.DragMove();

    private bool GetAdminState()
    {
        var winIdentity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(winIdentity);

        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
