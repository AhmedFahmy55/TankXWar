using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public enum AuthState
    {
        NotAuthenticated,
        Authenticating,
        Authenticated,
        Failed,
        TimeOut
    }



    public static AuthState AuthenticationState { get; private set; } = AuthState.NotAuthenticated;


    public static async Task<AuthState> DoAuth(int MaxTries = 5)
    {
        if (AuthenticationState == AuthState.Authenticated) return AuthenticationState;

        if(AuthenticationState == AuthState.Authenticating)
        {
            await Authenticating();
            return AuthenticationState;
        }

        await SignInAnonymously(MaxTries);

        return AuthenticationState;

    }

    private static async Task Authenticating()
    {
        while(AuthenticationState == AuthState.Authenticating || AuthenticationState == AuthState.NotAuthenticated)
        {
            await Task.Delay(1000);
        }
    }

    private static async Task SignInAnonymously(int MaxTries)
    {
        AuthenticationState = AuthState.Authenticating;

        int tries = 0;
        while (tries < MaxTries && AuthenticationState == AuthState.Authenticating)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthenticationState = AuthState.Authenticated;
                    break;

                }

                tries++;
                await Task.Delay(1000);
            }
            catch(AuthenticationException authEx)
            {
                Debug.Log(authEx);
                AuthenticationState = AuthState.Failed;
                break;
            }
            catch (RequestFailedException failedEx)
            {
                Debug.Log(failedEx);
                AuthenticationState = AuthState.Failed;
                break;
            }
        }

        if (AuthenticationState != AuthState.Authenticated) AuthenticationState = AuthState.TimeOut;
    }
}


