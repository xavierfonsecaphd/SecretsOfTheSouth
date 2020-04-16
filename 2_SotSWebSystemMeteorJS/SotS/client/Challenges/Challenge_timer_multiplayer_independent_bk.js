 
//Meteor.subscribe('Challenges_Hunter');  

import { Meteor } from 'meteor/meteor';
import './Challenge_timer_multiplayer_independent_bk.html';
import {markers} from './Challenges.js';
import { GoogleMaps } from 'meteor/dburles:google-maps';
import { Challenges_Timer_Multiplayer_IndependentDB } from '/Collections/Challenges_Timer_Multiplayer_Independent.js';


lat_addChallenge = new ReactiveVar(0.0);
lng_addChallenge = new ReactiveVar(0.0);
added_Location = new ReactiveVar(false);

var myMarker = null;
var listOfChallenges_timer_multiplayer_independent;


//console.log(Meteor.settings.public.ga.account);

Template.Challenge_timer_multiplayer_independent_bk.onCreated (function () {
    var self = this;

    this.editMode = new ReactiveVar (false);

    listOfChallenges_timer_multiplayer_independent = Challenges_Timer_Multiplayer_IndependentDB.find({});
    
    GoogleMaps.ready('UpdateMap', function(map) {

        
        listOfChallenges_timer_multiplayer_independent.forEach(element => 
        {
            if (element._id == this._id)
            {
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

});





Template.Challenge_timer_multiplayer_independent_bk.helpers({ 
    // Basicamente isto é um helper que vai colocar todas as receitas da BD dentro de "recipes"
    challenges_timer_multiplayer_independent() {
        return Challenges_Timer_Multiplayer_IndependentDB.find({});
      },
      challenge_timer_multiplayer_independent(id) {
        return Challenges_Timer_Multiplayer_IndependentDB.find({_id:id});
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


Template.Challenge_timer_multiplayer_independent_bk.events ({ 
    'click .fa-pencil': function(event, template) { 
         template.editMode.set(!Template.instance().editMode.get());  // this is a reactiveVar, meaning a session 
                                                         // variable that is attached to a template of 
                                                         // a website
     } , 

    'click .fa-trash': function() { 
        Meteor.call('deleteChallenge_Timer_Multiplayer_Independent', this._id);
   },

    'submit .info-challenge-edit_timer_multiplayer_independent'(event) {
        event.preventDefault();
        const target = event.target;
        const name = target.name;
        const description = target.description;
        const typeOfChallengeIndex = 4;
        const timer = target.timer;
        var lat;
        var lng;

        if ((lat_addChallenge.get() != 0.0) && (lng_addChallenge.get() != 0.0))
        {
                lat = lat_addChallenge.get();
                lng = lng_addChallenge.get();
        }
        else
        {
            //var chs = ChallengesDB.find({});
            listOfChallenges_timer_multiplayer_independent.forEach(element => 
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
        var timerPassed = parseInt (timer.value, 10);
        const ownerPlayFabID = Meteor.user().username;
        const route = target.route;
        if ((route.value != 0) && (route.value != 1) && (route.value != 2) && (route.value != 3)) {route.value = 0;}
        console.info('user id: ' + ownerPlayFabID);
        if (!!ownerPlayFabID )
        {
            if ((timerPassed < 1) || (timerPassed > 60)) {
                alert('The amount of time for any task should be between 1 and 60 minutes (1 hour)');
            }
            else {
                Meteor.call('updateChallenges_Timer_Multiplayer_Independent', this._id, 
                name.value, 
                description.value,
                ownerPlayFabID,
                typeOfChallengeIndex,
                lat,
                lng,
                task.value,
                imageURL.value,
                timerPassed,
                route.value,
                false, (error) => {
                    if (error) {
                        alert(error.error);
                    } else {
                        
                        alert("Timer Multiplayer Independent Challenge updated successfully :) Reloading page...");
                        window.location.reload(true); 
                    }
                });
    
                Template.instance().editMode.set(false);
            }
        }
        else
        {
            FlowRouter.go('/');
        }
            
        
      },
      'click .close-edit-challenge_timer_multiplayer_independent': function() { 
            Template.instance().editMode.set(false);
   }
});

