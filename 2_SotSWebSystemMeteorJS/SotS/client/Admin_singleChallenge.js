// Admin_singleChallenge
// Meteor.subscribe('challenges');  
import { ChallengesDB } from '/Collections/Challenges.js';
import { Meteor } from 'meteor/meteor';
import './Admin_singleChallenge.html';
import {markers} from './Admin.js';
import { GoogleMaps } from 'meteor/dburles:google-maps';
import { PlayFabClientSDK } from '/PlayFab/PlayFabClientApi.js';
import { PlayFabServerSDK } from '/PlayFab/PlayFabServerApi.js';
import { PlayFab } from '/PlayFab/PlayFabClientApi.js';




lat_addChallenge = new ReactiveVar(0.0);
lng_addChallenge = new ReactiveVar(0.0);
added_Location = new ReactiveVar(false);

var myMarker = null;
var listOfChallenges;


//console.log(Meteor.settings.public.ga.account);

Template.Admin_singleChallenge.onCreated (function () {
    var self = this;

    this.editMode = new ReactiveVar (false);
    this.validated = new ReactiveVar (false);

    listOfChallenges = ChallengesDB.find({});
    
    GoogleMaps.ready('UpdateMap', function(map) {

        //var chs = ChallengesDB.find({});
        listOfChallenges.forEach(element => 
                {
                    if (element._id == this._id)
                    {
                        // set the reactive var as true or false 
                        validated.set(element.validated);

                        var oneMarker = new google.maps.Marker({
                            draggable: false,
                            animation: google.maps.Animation.DROP,
                            position: new google.maps.LatLng(element.latitude, element.longitude),
                            map: map.instance,
                            // We store the document _id on the marker in order 
                            // to update the document within the 'dragend' event below.
                            id: document._id
                            });

                        map.instance.setCenter(new google.maps.LatLng(element.latitude, element.longitude));
                    }
                });


        google.maps.event.addListener(map.instance, 'click', function(event) {
            if (!added_Location.get())
            {
                lat_addChallenge.set (event.latLng.lat());
                lng_addChallenge.set (event.latLng.lng());
            
                // Clear the event listener
                if (myMarker != null)
                {
                    myMarker.setMap(null);
                    google.maps.event.clearInstanceListeners(myMarker);
                }
                

                    

                myMarker = new google.maps.Marker({
                    draggable: false,
                    animation: google.maps.Animation.DROP,
                    position: new google.maps.LatLng(event.latLng.lat(), event.latLng.lng()),
                    map: map.instance,
                    // We store the document _id on the marker in order 
                    // to update the document within the 'dragend' event below.
                    id: document._id
                    });

                    google.maps.event.addListener(myMarker, 'click', function(event) 
                    {
                        // Remove the marker from the map
                        myMarker.setMap(null);

                        // Clear the event listener
                        google.maps.event.clearInstanceListeners(myMarker);
                    });
            }

          });
     });


     console.log("On Load -> lat_addChallenge: " + lat_addChallenge.get() + "lng_addChallenge: " + lng_addChallenge.get());

});





Template.Admin_singleChallenge.helpers({ 
    // Basicamente isto é um helper que vai colocar todas as receitas da BD dentro de "recipes"
    challenges() {
        return ChallengesDB.find({});
      },
      challenge(id) {
        return ChallengesDB.find({_id:id});
      },
    // porque estás a usar variáveis, precisas de as colocar no helper
    editMode: function () {
        return Template.instance().editMode.get();  // we are returning THIS SPECIFIC variable 
                                                    // from this template
    },
    isOwnerUser: function(user){
        return Meteor.user().username == user;
    },
    typeOfChallengeString: function(index){
        if (index == 0)
        {
            return 'Quiz';
        }
        else if (index == 1)
        {
            return 'AR';
        }
        else if (index == 2)
        {
            return 'IoT';
        }
        else { return 'Other';}
    },
    lat_addChallenge: function () {
        return Template.instance().lat_addChallenge.get();
    },
    lng_addChallenge: function () {
        return Template.instance().lng_addChallenge.get();
    },
    mapOptions2: function() {
        if (GoogleMaps.loaded()) {
          return {
            center: new google.maps.LatLng(51.8944964, 4.5143659),
            zoom: 11
          };
        }
      }
}); 


Template.Admin_singleChallenge.events ({ 
    'click .fa-pencil': function(event, template) { 
         template.editMode.set(!Template.instance().editMode.get());  // this is a reactiveVar, meaning a session 
                                                         // variable that is attached to a template of 
                                                         // a website
     } , 

    'click .fa-trash': function() { 
        Meteor.call('deleteChallenge', this._id);
   },

    'submit .info-challenge-edit'(event) {
        event.preventDefault();
        const target = event.target;
    
        const name = target.name;
        const description = target.description;
        const typeOfChallengeIndex = target.typeOfChallengeIndex;
        var lat;
        var lng;

        console.log("lat_addChallenge: " + lat_addChallenge.get() + "lng_addChallenge: " + lng_addChallenge.get());

        if ((lat_addChallenge.get() != 0.0) && (lng_addChallenge.get() != 0.0))
        {
                lat = lat_addChallenge.get();
                lng = lng_addChallenge.get();
        }
        else
        {
            //var chs = ChallengesDB.find({});
            listOfChallenges.forEach(element => 
                {
                    if (element._id == this._id)
                    {
                        lat = element.latitude;
                        lng = element.longitude;
                    }
                });
             
            
        }

        const question = target.question;
        const answer = target.answer;
        const imageURL = target.imageURL;
        const route = target.route;
        if ((route.value != 0) && (route.value != 1) && (route.value != 2) && (route.value != 3)) {route.value = 0;}

        //var chs = ChallengesDB.find({});
        listOfChallenges.forEach(element => 
                {
                    if (element._id == this._id)
                    {
                        const ownerPlayFabID = element.ownerPlayFabID;
                        console.info('user id: ' + ownerPlayFabID);

                        Meteor.call('updateChallenge', this._id, 
                        name.value, 
                        description.value,
                        ownerPlayFabID,
                        typeOfChallengeIndex.value,
                        lat,
                        lng,
                        question.value,
                        answer.value,
                        imageURL.value,
                        route.value,
                        element.validated, 
                        (error) => {
                            if (error) {
                                alert(error.error);
                            } else {
                                window.location.reload(true); 
                                alert("Challenge update successfully :) Reloading page...");
                            }
                        });


                    }
                });
 
        
      },
      'click .close-edit-challenge': function() { 
            Template.instance().editMode.set(false);
   },
   'click .toToggleTheValidation': function () {


    var chs = ChallengesDB.find({});
    chs.forEach(element => 
        {
            if (element._id == this._id)
            {
                var validationStatus = element.validated;
                Meteor.call('toggleValidation', this._id, 
                validationStatus,
                (error) => {
                    if (error) {
                        alert(error.error);
                    } else {
                        if (validationStatus)
                        {
                            //alert("Challenge Revoked!");
                            ServerRemoveChallengeToPlayFabTitleData(element._id, element.ownerPlayFabID);
                            
                        }
                        else
                        {
                            //alert("Challenge Validated :)");
                            ServerAddChallengeToPlayFabTitleData(element._id, element.ownerPlayFabID);
                        }
                        
                    }
                });


            }
        });
   },
   'click .playfab_test': function() { 

    


     
   }
});

function ServerRemoveChallengeToPlayFabTitleData(thisId, ownerPlayFabID) {
    PlayFabServerSDK.GetTitleData({}, function (response, error) 
    {
        if(error) {
            console.log("Got error getting titleData:");
            console.log(PlayFab.GenerateErrorReport(error));
        } else {
            if(!response.data.Data || !response.data.Data[thisId]) {
                // first, create the new challenge
                
                PlayFabServerSDK.SetTitleData({
                    Key : thisId,
                    Value : "{\""+thisId+"\":[\"DISABLED\"]}"
                },
                function (response, error) {
                    if(error){
                        console.log("Got error setting titleData " + thisId + ":");
                        console.log(PlayFab.GenerateErrorReport(error));
                    } else 
                    {
                        console.log("" + thisId + " got successfully disabled.");
                        manageBadgeChallengesCreatedForOwner(false, ownerPlayFabID);
                        alert("Challenge Revoked!");
                        
                        //ServerRemoveChallengeFromListOfChallenges(thisId);                        
                    }
                    
                });
            }
            else {
                PlayFabServerSDK.SetTitleData({
                    Key : thisId,
                    Value : "{\""+thisId+"\":[\"DISABLED\"]}"
                },
                function (response, error) {
                    if(error){
                        console.log("Got error setting titleData " + thisId + ":");
                        console.log(PlayFab.GenerateErrorReport(error));
                    } else 
                    {
                        console.log("" + thisId + " got successfully disabled.");
                        manageBadgeChallengesCreatedForOwner(false, ownerPlayFabID);
                        alert("Challenge Revoked!");
                        
                        //ServerRemoveChallengeFromListOfChallenges(thisId);                        
                    }
                    
                });
                  
                
            }
            
        };

    });
}

function ServerAddChallengeToPlayFabTitleData (thisId, ownerPlayFabID) { 
    PlayFabServerSDK.GetTitleData({}, function (response, error) 
    {
        if(error) {
            console.log("Got error getting titleData:");
            console.log(PlayFab.GenerateErrorReport(error));
        } else {
            if(!response.data.Data || !response.data.Data[thisId]) {
                // first, create the new challenge
                //console.log("No Challenge 270036");
                PlayFabServerSDK.SetTitleData({
                    Key : thisId,
                    Value : "{\""+thisId+"\":[]}"
                },
                function (response, error) {
                    if(error){
                        console.log("Got error setting titleData " + thisId + ":");
                        console.log(PlayFab.GenerateErrorReport(error));
                    } else 
                    {
                        console.log("Set titleData successful for " + thisId);

                        //ServerUpdateListOfChallengesWithArray(thisId);    
                        alert("Challenge Validated :)");        
                        
                        manageBadgeChallengesCreatedForOwner(true, ownerPlayFabID);
                    }
                    
                });
            }
            else {
                console.log("Challenge found: "+response.data.Data[thisId]);
                PlayFabServerSDK.SetTitleData({
                    Key : thisId,
                    Value : "{\""+thisId+"\":[]}"
                },
                function (response, error) {
                    if(error){
                        console.log("Got error setting titleData " + thisId + ":");
                        console.log(PlayFab.GenerateErrorReport(error));
                    } else 
                    {
                        console.log("Set titleData successful for " + thisId);

                        alert("Challenge Validated :)");
                        //ServerUpdateListOfChallengesWithArray(thisId);      
                        manageBadgeChallengesCreatedForOwner(true, ownerPlayFabID);                  
                    }
                    
                });
                
            }
            
        };

    });

}


// increment true to add one point, false to decrement one point
function manageBadgeChallengesCreatedForOwner(increment, ownerPlayFabID) 
{
    PlayFabServerSDK.GetPlayerStatistics({
        PlayFabId : ownerPlayFabID
    }, function (response, error) 
    {
        if(error) {
            console.log("Got error requesting for Player's statistics:");
            console.log(PlayFab.GenerateErrorReport(error));
        } else 
        {     
            var length = response.data.Statistics.length;

            var count;
            var found = false; // if at the end of the parsing this is still false, you should add this statistic!
            for (count = 0;count < length; count++)
            {
                //var obj = JSON.parse(response.data.Statistics[count]);   
                console.log("Statistics [" + count + "] :"+response.data.Statistics[count]["StatisticName"]);
                // ChallengesCreated
                if (new String(response.data.Statistics[count]["StatisticName"]).valueOf() == new String("ChallengesCreated").valueOf())
                {
                    var chsCreated;
                    if (increment)
                    {
                        var newValue = response.data.Statistics[count]["Value"] + 1;
                        chsCreated= { StatisticName: "ChallengesCreated", Value: newValue };
                    }
                    else 
                    {
                        if (response.data.Statistics[count]["Value"] > 0)
                        {
                            var newValue = response.data.Statistics[count]["Value"] - 1;
                            chsCreated= { StatisticName: "ChallengesCreated", Value: newValue };
                        }
                        else 
                        {chsCreated= { StatisticName: "ChallengesCreated", Value: 0 };}
                    }
                    
                    found = true;
                    
                }
            }

            if (!found) // otherwise, we did not find such statistic. add it anyhow
            {
                if (increment)
                {
                    var chsCreated = { StatisticName: "ChallengesCreated", Value: 1 };
                }
                else 
                {
                    var chsCreated = { StatisticName: "ChallengesCreated", Value: 0 };   
                }
            }
            
            PlayFabServerSDK.UpdatePlayerStatistics({
                PlayFabId: ownerPlayFabID,
                Statistics: [chsCreated]
            }, function (response, error) 
            {
                if(error) {
                    console.log("Got error updating for Player's statistics:");
                    console.log(PlayFab.GenerateErrorReport(error));
                } else 
                { 
                    console.log("Success at updating the number of Challenges created for " + ownerPlayFabID);
                }
            });
        }
    
    });
}




/*
function ServerRemoveChallengeFromListOfChallenges(thisId) {
    PlayFabServerSDK.GetTitleData({}, function (response, error) 
    {
        if(error) {
            console.log("Got error getting titleData:");
            console.log(PlayFab.GenerateErrorReport(error));
        } else {
            if(!response.data.Data || !response.data.Data["ListOfChallenges"]) {
                // first, create the new challenge
                console.log("No List of Challenges");
                // build vector of challenges
                
                var valueToSubmitForListOfChallenges = "{\"ListOfChallenges\":[]}";
                PlayFabServerSDK.SetTitleData({
                    Key : "ListOfChallenges",
                    Value : valueToSubmitForListOfChallenges
                },
                function (response, error) {
                    if(error){
                        console.log("Got error setting ListOfChallenges point 1 with " + valueToSubmitForListOfChallenges + ":");
                        console.log(PlayFab.GenerateErrorReport(error));
                    } else 
                    {
                        console.log("Set titleData successful for " + valueToSubmitForListOfChallenges);
                    }
                    
                });
            }
            else {
                
                console.log("List Of Challenges: "+response.data.Data["ListOfChallenges"]);
                
                var obj = JSON.parse(response.data.Data["ListOfChallenges"]);
                var valueToSubmitForListOfChallenges = "{\"ListOfChallenges\":[";
                var resultArray = [];  
                  Object.keys(obj).forEach(function(key) 
                  {
                    var i, len;
                    for (len = obj[key].length, i=0; i<len; ++i) {  
                        if (new String(obj[key][i]).valueOf() != new String(thisId).valueOf())
                        {
                            resultArray.push(obj[key][i]);
                        }
                    }

                  });

                  if (resultArray.length >= 2)
                    {
                        for (var j = 0; j < resultArray.length - 1; j++)
                        {
                            valueToSubmitForListOfChallenges += "\"";
                            valueToSubmitForListOfChallenges += resultArray[j];
                            valueToSubmitForListOfChallenges += "\",";
                        }
    
                        valueToSubmitForListOfChallenges += "\"";
                        valueToSubmitForListOfChallenges += resultArray[resultArray.length - 1];
                        valueToSubmitForListOfChallenges += "\"]}";
                    }
                    else if (resultArray.length == 0)
                    {
                        valueToSubmitForListOfChallenges = "{\"ListOfChallenges\":[]}";   
                    }
                    else 
                    {
                        valueToSubmitForListOfChallenges = "{\"ListOfChallenges\":[\"" +resultArray[0] +"\"]}";   
                    }


                  PlayFabServerSDK.SetTitleData({
                    Key : "ListOfChallenges",
                    Value : valueToSubmitForListOfChallenges
                },
                function (response, error) {
                    if(error){
                        console.log("Got error setting ListOfChallenges point 2 with: " + valueToSubmitForListOfChallenges);
                        console.log(PlayFab.GenerateErrorReport(error));
                    } else console.log("Set titleData successful for ListOfChallenges");
                });
                
            }


            alert("Challenge Revoked!");
            manageBadgeChallengesCreatedForOwner(false, ownerPlayFabID);
            
        };

    });
}
*/

/*
var obj = JSON.parse(response.data.Data["ListOfChallenges"]);
                var valueToSubmitForListOfChallenges = "{\"ListOfChallenges\":[";
                var resultArray = [];  
                  Object.keys(obj).forEach(function(key) 
                  {
                    var i, len;
                    for (len = obj[key].length, i=0; i<len; ++i) {

                        
                        if (new String(obj[key][i]).valueOf() == new String(thisId).valueOf())
                        {resultArray.push(obj[key][i]);}
                    }
                });

                    if (resultArray.length >= 2)
                    {
                        for (var j = 0; j < resultArray.length - 1; j++)
                        {
                            valueToSubmitForListOfChallenges += "\"";
                            valueToSubmitForListOfChallenges += resultArray[j];
                            valueToSubmitForListOfChallenges += "\",";
                        }
    
                        valueToSubmitForListOfChallenges += "\"";
                        valueToSubmitForListOfChallenges += resultArray[resultArray.length - 1];
                        valueToSubmitForListOfChallenges += "\"]}";
                    }
                    else 
                    {
                     valueToSubmitForListOfChallenges = "{\"ListOfChallenges\":[]}";   
                    }

                


                PlayFabServerSDK.SetTitleData({
                    Key : "ListOfChallenges",
                    Value : valueToSubmitForListOfChallenges
                },
                function (response, error) {
                    if(error){
                        console.log("Got error setting ListOfChallenges point 2 with: " + valueToSubmitForListOfChallenges);
                        console.log(PlayFab.GenerateErrorReport(error));
                    } else console.log("Set titleData successful for ListOfChallenges");
                });
*/



function ServerUpdateListOfChallengesWithArray (thisId) {
    PlayFabServerSDK.GetTitleData({}, function (response, error) 
    {
        if(error) {
            console.log("Got error getting titleData:");
            console.log(PlayFab.GenerateErrorReport(error));
        } else {
            if(!response.data.Data || !response.data.Data["ListOfChallenges"]) {
                // first, create the new challenge
                console.log("No List of Challenges");
                // build vector of challenges
                
                var valueToSubmitForListOfChallenges = "{\"ListOfChallenges\":[\"" + thisId +"\"]}";
                PlayFabServerSDK.SetTitleData({
                    Key : "ListOfChallenges",
                    Value : valueToSubmitForListOfChallenges
                },
                function (response, error) {
                    if(error){
                        console.log("Got error setting ListOfChallenges point 1 with " + valueToSubmitForListOfChallenges + ":");
                        console.log(PlayFab.GenerateErrorReport(error));
                    } else 
                    {
                        console.log("Set titleData successful for " + valueToSubmitForListOfChallenges);
                    }
                    
                });
            }
            else {
                console.log("List Of Challenges: "+response.data.Data["ListOfChallenges"]);
                
                var obj = JSON.parse(response.data.Data["ListOfChallenges"]);
                var valueToSubmitForListOfChallenges = "{\"ListOfChallenges\":[";
                var resultArray = [];  
                var found = false;
                  Object.keys(obj).forEach(function(key) 
                  {
                    var i, len;
                    for (len = obj[key].length, i=0; i<len; ++i) {
                        if (new String(obj[key][i]).valueOf() != new String(thisId).valueOf())
                        {
                            resultArray.push(obj[key][i]);
                        }
                        else 
                        {
                            found = true;
                        }
                    }
                });

                // only makes sense to add if the value is not there
                if (!found)
                {


                    if (resultArray.length == 0)
                    {
                        valueToSubmitForListOfChallenges = "{\"ListOfChallenges\":[\"" +thisId +"\"]}";   
                    }
                    else {
                        for (var j = 0; j < resultArray.length; j++)
                        {
                            valueToSubmitForListOfChallenges += "\"";
                            valueToSubmitForListOfChallenges += resultArray[j];
                            valueToSubmitForListOfChallenges += "\",";
                        }

                        valueToSubmitForListOfChallenges += "\"";
                        valueToSubmitForListOfChallenges += thisId;
                        valueToSubmitForListOfChallenges += "\"]}";

                    }






                    PlayFabServerSDK.SetTitleData({
                        Key : "ListOfChallenges",
                        Value : valueToSubmitForListOfChallenges
                    },
                    function (response, error) {
                        if(error){
                            console.log("Got error setting ListOfChallenges point 2 with: " + valueToSubmitForListOfChallenges);
                            console.log(PlayFab.GenerateErrorReport(error));
                        } else console.log("Set titleData successful for ListOfChallenges");
                    });
                }

                alert("Challenge Validated :)");
                
            }
            
        };

    });
}

/*

// then, update the list of challenges
                        if(!response.data.Data || !response.data.Data["ListOfChallenges"]) 
                        {
                        //    console.log("Challenge: "+response.data.Data["ListOfChallenges"]);
                            var obj2 = JSON.parse(response.data.Data["ListOfChallenges"]);
                            var resultArray2 = [];  
                            Object.keys(obj2).forEach(function(key) 
                            {
                                var i, len;
                                for (len = obj2[key].length, i=0; i<len; ++i) {
                                    resultArray2.push(obj2[key][i]);
                                }

                            });

                            // build vector of challenges
                            var valueToSubmitForListOfChallenges;
                            if (resultArray2.length == 0)
                            {
                                valueToSubmitForListOfChallenges = "{\"ListOfChallenges\":[]}";
                            }
                            else
                            {
                                valueToSubmitForListOfChallenges = "{\"ListOfChallenges\":[";
                                var i, len;
                                for (len = obj2[key].length, i=0; i<len - 1; ++i) {
                                    valueToSubmitForListOfChallenges += "\"";
                                    valueToSubmitForListOfChallenges += resultArray2[i];
                                    valueToSubmitForListOfChallenges += "\",";
                                }
                                valueToSubmitForListOfChallenges += "\"";
                                valueToSubmitForListOfChallenges += resultArray2[len - 1];
                                valueToSubmitForListOfChallenges += "\"]}";

                                PlayFabServerSDK.SetTitleData({
                                    Key : "ListOfChallenges",
                                    Value : valueToSubmitForListOfChallenges
                                },
                                function (response, error) {
                                    if(error){
                                        console.log("Got error setting ListOfChallenges with: " + valueToSubmitForListOfChallenges);
                                        console.log(PlayFab.GenerateErrorReport(error));
                                    } else console.log("Set titleData successful for ListOfChallenges");
                                });
                            }
                        }

*/