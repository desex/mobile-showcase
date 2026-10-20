#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

extern "C"
{
    char* MakeStringCopy(const char* string)
    {
        if (!string) return NULL;
        char* res = (char*)malloc(strlen(string) + 1);
        strcpy(res, string);
        return res;
    }

    const char* GetSettingsURL()
    {
        NSURL* url = [NSURL URLWithString:UIApplicationOpenSettingsURLString];
        return MakeStringCopy([[url absoluteString] UTF8String]);
    }

    void OpenSettings()
    {
        NSURL* url = [NSURL URLWithString:UIApplicationOpenSettingsURLString];

        if (@available(iOS 10.0, *))
        {
            [[UIApplication sharedApplication] openURL:url
                                               options:@{}
                                     completionHandler:nil];
        }
        else
        {
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
            [[UIApplication sharedApplication] openURL:url];
#pragma clang diagnostic pop
        }
    }
}
