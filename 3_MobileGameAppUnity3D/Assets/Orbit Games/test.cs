using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    public bool show;
    [ConditionalField("show", compareValue: true)]
    public BoolEvent events;

    [Header("Loading")]
    [Buttons("ShowLoading", "ShowLoading", "HideLoading", "HideLoading")]
    public ButtonsContainer loading;
    public string loadingText = "Loading";

    public void ShowLoading()
    {
        OGLoadingOverlay.ShowFullcoverLoading(loadingText);
    }

    public void HideLoading()
    {
        OGLoadingOverlay.StopOldestActiveLoading();
    }


    [Header("Popup")]
    [Buttons("Random", "PopupTest", "Form", "PopupTest2", "Texture", "PopupTest3")]
    public ButtonsContainer popup;
    public Texture2D[] texturesToTest;

    private int testPopup = 0;
    public void PopupTest()
    {
        switch (testPopup)
        {
            case 0:
                OGPopup.MakeNotificationPopup(("Bliep1"), ("bloep1"), () => Debug.Log("Closed notifiation")).Show();
                break;
            case 1:
                OGPopup.MakeWarningPopup(("Bliep2"), ("bloep2"), () => Debug.Log("Closed warning")).Show();
                break;
            case 2:
                OGPopup.MakeErrorPopup(("Bliep3"), ("bloep3"), () => Debug.Log("Closed error")).Show();
                break;
            case 3:
                OGPopup.MakeConfirmPopup(("Bliep4"), ("Would you bloep4?"), () => Debug.Log("Accepted!"), () => Debug.Log("Denied!")).Show();
                break;
            case 4:
                OGPopup.MakeConfirmPopup(("Bliep4"), ("Would you bloep4?"), () => Debug.Log("Accepted!"), () => Debug.Log("Denied!")).Show();
                break;
            case 5:
                var prompt = OGPopup.MakePromptPopup(("Bliep5"), ("Would you fill in the data below please"), (data) => Debug.Log(data), () => Debug.Log("Canceled"));
                prompt.formHelper.AddTextField(("Some bloop"), "a_variable");
                prompt.Show();
                break;
            case 6:
                var prompt2 = OGPopup.MakePromptPopup(("Bliep5"), ("Would you fill in the data below please"), (data) => Debug.Log(data), () => Debug.Log("Canceled"));
                prompt2.formHelper.AddTextField(("Some bloop"), "a_variable");
                prompt2.Show();
                break;
            default:
                break;
        }

        testPopup++;
        testPopup = testPopup % 7;
    }

    public void PopupTest2()
    {
        var prompt = OGPopup.MakePromptPopup(("Bliep5"), ("Would you fill in the data below please"), (data) => Debug.Log(data), () => Debug.Log("Canceled"));
        prompt.formHelper.AddTextArea(("Some bloop"), "a_variable", null, 500);
        prompt.formHelper.AddHeader(("HEader"));
        prompt.formHelper.AddPasswordField(("Some bloop"), "a_password");
        prompt.formHelper.AddCheckbox(("HEader"), "another_variable");
        prompt.AddClosingButton("New random button", () => { Debug.Log("Wow"); });
        prompt.Show();

        prompt.SetData("a_variable", "This is some value filled in the form");
    }

    public void PopupTest3()
    {
        var prompt2 = OGPopup.MakePromptPopup(("Bliep5"), ("Would you fill in the data below please"), (data) => Debug.Log(data), () => Debug.Log("Canceled"));
        prompt2.formHelper.AddTextField(("Some bloop"), "a_variable");
        prompt2.SetTextureInsteadOfIcon(texturesToTest[Random.Range(0, texturesToTest.Length)]);
        prompt2.Show();
    }
}
