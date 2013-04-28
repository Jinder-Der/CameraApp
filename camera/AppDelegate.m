//
//  AppDelegate.m
//  camera
//
//  Created by Owen Evans on 24/04/13.
//  Copyright (c) 2013 Owen Evans. All rights reserved.
//

#import "AppDelegate.h"
#import "Application.h"
#import "MainMenuViewController.h"
#import "DDASLLogger.h"
#import "DDTTYLogger.h"
#import "TestFlightLogger.h"
#import "TestFlight.h"
#import <Crashlytics/Crashlytics.h>

@implementation AppDelegate

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    

    [TestFlight setDeviceIdentifier:[[UIDevice currentDevice] uniqueIdentifier]];

    [TestFlight takeOff:@"36e40e5d-ae3d-47da-bb3a-9d445c52c367"];

    
    [Crashlytics startWithAPIKey:@"076e472d795137f4df4b5d12ec7cea788958e843"];
    
    [DDLog addLogger:[TestFlightLogger sharedInstance]];

    [DDLog addLogger:[DDASLLogger sharedInstance]];
    [DDLog addLogger:[DDTTYLogger sharedInstance]];
    DDLogVerbose(@"loading application");
    

    self.window = [[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    // Override point for customization after application launch.
    [application setStatusBarHidden:YES];
    DDLogVerbose(@"setting root view controller");
    [[self window] setRootViewController:[[MainMenuViewController alloc] initWithNibName:@"MainMenuView" bundle:[NSBundle mainBundle]]];
    [self.window makeKeyAndVisible];
    [Application initialize];
    DDLogVerbose(@"raising load notification");
    [[NSNotificationCenter defaultCenter] postNotificationName:kApplicationLoadedNotification object:nil];
    DDLogVerbose(@"returning from application did load");

  
    return YES;
}

@end