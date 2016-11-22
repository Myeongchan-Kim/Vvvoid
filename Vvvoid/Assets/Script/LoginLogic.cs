/*
 * Copyright (C) 2014 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginLogic : MonoBehaviour
{
    public Button authButton;
    public Text statusText;
    public bool omitLogin;
    private const float FontSizeMult = 0.05f;
    private bool mWaitingForAuth = false;
    private bool dumpedToken = false;

    void Start()
    {
        if(omitLogin)
        {
            SceneManager.LoadScene("GameScene");
        }
        // Select the Google Play Games platform as our social platform implementation
        GooglePlayGames.PlayGamesPlatform.Activate();



        var buttonText = authButton.GetComponentInChildren<Text>();

        buttonText.text = "Authenticate";
    }

    public void OnButtonClick()
    {

        if (!Social.localUser.authenticated)
        {
            // Authenticate
            mWaitingForAuth = true;
            statusText.text = "Authenticating...";
            Social.localUser.Authenticate((bool success) =>
            {
                mWaitingForAuth = false;
                if (success)
                {
                    statusText.text = "Welcome " + Social.localUser.userName;

                    SceneManager.LoadScene("GameScene");
                }
                else
                {
                    statusText.text = "Authentication failed.";
                }
            });
        }
    }
    
}
