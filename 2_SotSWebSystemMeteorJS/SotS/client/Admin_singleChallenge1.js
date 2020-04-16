// Admin_singleChallenge
// Meteor.subscribe('challenges');  
import { Challenges1DB } from '/Collections/Challenges1.js';
import { Meteor } from 'meteor/meteor';
import './Admin_singleChallenge1.html';
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

Template.Admin_singleChallenge1.onCreated (function () {
    var self = this;

    this.editMode = new ReactiveVar (false);
    this.validated = new ReactiveVar (false);

    listOfChallenges = Challenges1DB.find({});
    
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





Template.Admin_singleChallenge1.helpers({ 
    // Basicamente isto é um helper que vai colocar todas as receitas da BD dentro de "recipes"
    challenges1() {
        return Challenges1DB.find({});
      },
      challenge1(id) {
        return Challenges1DB.find({_id:id});
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
            return 'Multiplayer';
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


Template.Admin_singleChallenge1.events ({ 
    'click .fa-pencil': function(event, template) { 
         template.editMode.set(!Template.instance().editMode.get());  // this is a reactiveVar, meaning a session 
                                                         // variable that is attached to a template of 
                                                         // a website
     } , 

    'click .fa-trash': function() { 
        Meteor.call('deleteChallenge1', this._id);
   },

    'submit .info-challenge-edit1'(event) {
        event.preventDefault();
        const target = event.target;
    
        const name = target.name;
        const description = target.description;
        const typeOfChallengeIndex = 1;
        var lat;
        var lng;
        const route = target.route;
        if ((route.value != 0) && (route.value != 1) && (route.value != 2) && (route.value != 3)) {route.value = 0;}

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

        const task = target.task;
        const imageURL = target.imageURL;

        //var chs = ChallengesDB.find({});
        listOfChallenges.forEach(element => 
                {
                    if (element._id == this._id)
                    {
                        const ownerPlayFabID = element.ownerPlayFabID;
                        console.info('user id: ' + ownerPlayFabID);

                        Meteor.call('updateChallenge1', this._id, 
                        name.value, 
                        description.value,
                        ownerPlayFabID,
                        typeOfChallengeIndex,
                        lat,
                        lng,
                        task.value,
                        imageURL.value,
                        route.value,
                        element.validated, 
                        (error) => {
                            if (error) {
                                alert(error.error);
                            } else {
                                window.location.reload(true); 
                                alert("Multiplayer Challenge updated successfully :) Reloading page...");
                            }
                        });


                    }
                });
 
        
      },
      'click .close-edit-challenge1': function() { 
            Template.instance().editMode.set(false);
   },
   'click .toToggleTheValidation1': function () {


    var chs = Challenges1DB.find({});
    chs.forEach(element => 
        {
            if (element._id == this._id)
            {
                var validationStatus = element.validated;
                Meteor.call('toggleValidation1', this._id, 
                validationStatus,
                (error) => {
                    if (error) {
                        alert(error.error);
                    } else {
                        if (validationStatus)
                        {
                            //alert("Challenge Revoked!");
                            ServerRemoveChallenge1ToPlayFabTitleData(element._id, element.ownerPlayFabID);
                            
                        }
                        else
                        {
                            //alert("Challenge Validated :)");
                            ServerAddChallenge1ToPlayFabTitleData(element._id, element.ownerPlayFabID);
                        }
                        
                    }
                });


            }
        });
   }
});

function ServerRemoveChallenge1ToPlayFabTitleData(thisId, ownerPlayFabID) {
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
                        manageBadgeChallenges1CreatedForOwner(false, ownerPlayFabID);
                        alert("Multiplayer Challenge Revoked!");
                        
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
                        manageBadgeChallenges1CreatedForOwner(false, ownerPlayFabID);
                        alert("Multiplayer Challenge Revoked!");
                        
                        //ServerRemoveChallengeFromListOfChallenges(thisId);                        
                    }
                    
                });
                  
                
            }
            
        };

    });
}

function ServerAddChallenge1ToPlayFabTitleData (thisId, ownerPlayFabID) { 
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
                        alert("Multiplayer Challenge Validated :)");        
                        
                        manageBadgeChallenges1CreatedForOwner(true, ownerPlayFabID);
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

                        alert("Multiplayer Challenge Validated :)");
                        //ServerUpdateListOfChallengesWithArray(thisId);      
                        manageBadgeChallenges1CreatedForOwner(true, ownerPlayFabID);                  
                    }
                    
                });
                
            }
            
        };

    });

}


// increment true to add one point, false to decrement one point
function manageBadgeChallenges1CreatedForOwner(increment, ownerPlayFabID) 
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
