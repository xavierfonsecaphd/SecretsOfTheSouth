import { ChallengesDB } from '/Collections/Challenges.js';
import { Challenges1DB } from '/Collections/Challenges1.js';
import { Challenges_HunterDB } from '/Collections/Challenges_Hunter.js';
import { Challenges_VotingDB } from '/Collections/Challenges_Voting.js';
import { Challenges_Timer_Multiplayer_IndependentDB } from '/Collections/Challenges_Timer_Multiplayer_Independent.js';
import { Challenges_Timed_TaskDB } from '/Collections/Challenges_Timed_Task.js';
import { Challenges_OpenQuizDB } from '/Collections/Challenges_OpenQuiz.js';
import { Markers } from '/Collections/Markers.js';
import { Meteor } from 'meteor/meteor';
import './Admin.html';
import { PlayFabClientSDK } from '/PlayFab/PlayFabClientApi.js';
import { PlayFab } from '/PlayFab/PlayFabClientApi.js';

Template.Admin.onCreated (function () {

    this.editMode = new ReactiveVar (false);
    var self = this;
    
    self.autorun (function() {
        self.subscribe('challenges');
        self.subscribe('challenges1');
        self.subscribe('Challenges_Hunter');
        self.subscribe('Challenges_Voting');
        self.subscribe('Challenges_Timer_Multiplayer_Independent');
        self.subscribe('Challenges_Timed_Task');
        self.subscribe('Challenges_OpenQuiz');
    });


    // automatic login at PlayFab
    PlayFab.settings.titleId = '336E0';
    var loginRequest = {
        // Currently, you need to look up the correct format for this object in the API-docs:
        // https://api.playfab.com/documentation/Client/method/LoginWithCustomID
        TitleId: PlayFab.settings.titleId,
        Email: 'yourEmail@emailProvider.com',
        Password: 'administrator'
    };
    
    PlayFabClientSDK.LoginWithEmailAddress(loginRequest, function (result, error) 
    {
        if (result !== null) {
            console.log('Congratulations, The Admin is logged in.');
            //document.getElementById("resultOutput").innerHTML = "Congratulations, you made your first successful API call!";
        } else if (error !== null) {
            console.log('Error loggin into Playfab, '+PlayFab.GenerateErrorReport(error));
    
        }

    });
});



// ******************************************************
// ******************* Helpers **************************
// ******************************************************

Template.Admin.helpers({ 
    challenges() {
        return ChallengesDB.find({});
    },
    challenges1() {
        return Challenges1DB.find({});
    },
    Challenges_Hunter() {
        return Challenges_HunterDB.find({});
    },
    Challenges_Voting() {
        return Challenges_VotingDB.find({});
    },
    Challenges_Timer_Multiplayer_Independent() {
        return Challenges_Timer_Multiplayer_IndependentDB.find({});
    },
    Challenges_Timed_task() {
        return Challenges_Timed_TaskDB.find({});
    },
    Challenges_OpenQuiz() {
        return Challenges_OpenQuizDB.find({});
    },
    typeOfChallengeString: function(index){
        if (index == 0)
        {
            return 'Quiz';
        }
        else if (index == 1)
        {
            return 'Multiplayer';
        }
        else if (index == 2)
        {
            return 'Hunter';
        }
        else if (index == 3)
        {
            return 'Voting';
        }
        else if (index == 4)
        {
            return 'Timer Multiplayer (Ind.)';
        }
        else if (index == 5)
        {
            return 'Timed Task';
        }
        else if (index == 6)
        {
            return 'Open Quiz';
        }
        else { return 'Other';}
    },
    isOwnerUser: function(user){
        return Meteor.user().username == user;
    },
    editMode: function () {
        return Template.instance().editMode.get();  // we are returning THIS SPECIFIC variable 
                                                    // from this template
    }
});

Template.Admin.events ({

    'click .fa-trash': function() { 
        Meteor.call('deleteChallenge', this._id);
    },
    'click .fa-pencil': function(event, template) { 
        template.editMode.set(!Template.instance().editMode.get());  // this is a reactiveVar, meaning a session 
                                                        // variable that is attached to a template of 
                                                        // a website
    }

});