// Meteor.subscribe('challenges');  
import { ChallengesDB } from '/Collections/Challenges.js';
import { Challenges1DB } from '/Collections/Challenges1.js';
import { Markers } from '/Collections/Markers.js';
import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';
import { PlayFabClientSDK } from '/PlayFab/PlayFabClientApi.js';
import { PlayFab } from '/PlayFab/PlayFabClientApi.js';
import './Challenges.html';
import { Challenges_HunterDB } from '/Collections/Challenges_Hunter.js';
import { Challenges_VotingDB } from '/Collections/Challenges_Voting.js';
import { Challenges_Timer_Multiplayer_IndependentDB } from '/Collections/Challenges_Timer_Multiplayer_Independent.js';
import { Challenges_Timed_TaskDB } from '/Collections/Challenges_Timed_Task.js';
import { Challenges_OpenQuizDB } from '/Collections/Challenges_OpenQuiz.js';
import { Email } from 'meteor/email';

// noreply_secretsofthesouth@sapo.pt |  noreply_secretsofthesouth@yahoo.com
// MAIL_URL = 'smtp://USERNAME:PASSWORD@HOST:PORT';
// smtp.mail.yahoo.com
//MAIL_URL = 'smtps://noreply_secretsofthesouth@yahoo.com:SecretsOfTheSouth_sots0@smtp.mail.yahoo.com:465';
// smtp.mail.yahoo.com:465';

lat_addChallenge = new ReactiveVar(0.0);
lng_addChallenge = new ReactiveVar(0.0);
added_Location = new ReactiveVar(false);

var listOfChallenges;
var listOfChallenges1;
var listOfChallenges_hunter;
var listOfChallenges_voting;
var listOfChallenges_timer_multiplayer_independent;
var listOfChallenges_openQuiz;
var infoswindows = [];
var theMarker = null;

//console.log(Meteor.settings.public.ga.account);

Template.Challenges.onCreated (function () {
    
    this.introduceChallenge1 = new ReactiveVar(false);
    this.introduceChallengeHunter = new ReactiveVar(false);
    this.introduceChallengeVoting = new ReactiveVar(false);
    this.introduceChallengeTimerMultiplayerIndependent = new ReactiveVar (false);
    this.introduceChallengeTimedTask = new ReactiveVar(false);
    this.introduceChallengeOpenQuiz =  new ReactiveVar(false);

    this.sharedReactiveParentVar = new ReactiveVar(false);
    this.newChallengeMode = new ReactiveVar(true);
    this.showOverallMap = new ReactiveVar(false);

    this.editMode = new ReactiveVar (false);
    var self = this;
    
    /*self.autorun (function() {
        self.subscribe('challenges');
    });*/
    listOfChallenges = ChallengesDB.find({}); 
    listOfChallenges1 = Challenges1DB.find({}); 
    listOfChallenges_hunter = Challenges_HunterDB.find({});
    listOfChallenges_voting = Challenges_VotingDB.find({});
    listOfChallenges_timer_multiplayer_independent = Challenges_Timer_Multiplayer_IndependentDB.find({});
    listOfChallenges_timed_task = Challenges_Timed_TaskDB.find({});
    listOfChallenges_openQuiz = Challenges_OpenQuizDB.find({});
    

    GoogleMaps.ready('map', function(map) {
        
        google.maps.event.addListener(map.instance, 'click', function(event) {
            if (!added_Location.get())
            {
                lat_addChallenge.set (event.latLng.lat());
                lng_addChallenge.set (event.latLng.lng());
            
                // Clear the event listener
                if (theMarker != null)
                {
                    theMarker.setMap(null);
                    google.maps.event.clearInstanceListeners(theMarker);
                }
                


                theMarker = new google.maps.Marker({
                    draggable: false,
                    animation: google.maps.Animation.DROP,
                    position: new google.maps.LatLng(event.latLng.lat(), event.latLng.lng()),
                    map: map.instance,
                    // We store the document _id on the marker in order 
                    // to update the document within the 'dragend' event below.
                    id: document._id
                    });

                google.maps.event.addListener(theMarker, 'click', function(event) {
                    // Remove the marker from the map
                    theMarker.setMap(null);

                    // Clear the event listener
                    google.maps.event.clearInstanceListeners(theMarker);
                    });
                }

          });
     });

     
     Template.instance().showOverallMap.set(true);

     

    GoogleMaps.ready('OverallMap', function(map) {

        /**
         * Type of Challenges: Quiz
         */
 
        listOfChallenges.forEach(element => 
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
                   
                 
            google.maps.event.addListener(oneMarker, 'click', (function (event) 
            {
                var infowin;
                if (element.typeOfChallengeIndex == 0)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Quiz'
                    })
                }
                else if (element.typeOfChallengeIndex == 1)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Multiplayer'
                    })
                }
                else if (element.typeOfChallengeIndex == 2)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Hunter'
                    })
                }
                else if (element.typeOfChallengeIndex == 3)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Voting'
                    })
                }
                else if (element.typeOfChallengeIndex == 4)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timer Multiplayer (Ind.)'
                    })
                }
                else if (element.typeOfChallengeIndex == 5)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timed Task'
                    })
                }
                else if (element.typeOfChallengeIndex == 6)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Open Quiz'
                    })
                }
                else {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Other'
                    })
                }

                infoswindows.push(infowin);

                for(var i = 0; i < infoswindows.length; i++)
                {
                    infoswindows[i].close();
                }
                
                infowin.open(map, oneMarker);    
            }));
         });

        /**
         * Type of challenge: Multiplayer with Facilitators
         */

        var image = {
            url: '../images/MultiplayerIcon.png',// image is in root directory of meteor project.
            size: new google.maps.Size(71, 71),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(35, 35)
            };

        listOfChallenges1.forEach(element => 
        {

            var oneMarker = new google.maps.Marker({
                draggable: false,
                animation: google.maps.Animation.DROP,
                position: new google.maps.LatLng(element.latitude, element.longitude),
                map: map.instance,
                icon: image,
                // We store the document _id on the marker in order 
                // to update the document within the 'dragend' event below.
                id: document._id
                });
                      
                    
            google.maps.event.addListener(oneMarker, 'click', (function (event) 
            {
                var infowin;
                if (element.typeOfChallengeIndex == 0)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Quiz'
                    })
                }
                else if (element.typeOfChallengeIndex == 1)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Multiplayer'
                    })
                }
                else if (element.typeOfChallengeIndex == 2)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Hunter'
                    })
                }
                else if (element.typeOfChallengeIndex == 3)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Voting'
                    })
                }
                else if (element.typeOfChallengeIndex == 4)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timer Multiplayer (Ind.)'
                    })
                }
                else if (element.typeOfChallengeIndex == 5)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timed Task'
                    })
                }
                else if (element.typeOfChallengeIndex == 6)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Open Quiz'
                    })
                }
                //Challenges_Timed_TaskDB
                else {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Other'
                    })
                }

                infoswindows.push(infowin);

                for(var i = 0; i < infoswindows.length; i++)
                {
                    infoswindows[i].close();
                }
                
                infowin.open(map, oneMarker);
            }));
        });


        /**
         * Type of challenges: Hunter Challenge
         */
        var image_hunter = {
            url: '../images/huntericon.png',// image is in root directory of meteor project.
            size: new google.maps.Size(71, 71),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(35, 35)
            };

        listOfChallenges_hunter.forEach(element => 
        {
            var oneMarker = new google.maps.Marker({
                draggable: false,
                animation: google.maps.Animation.DROP,
                position: new google.maps.LatLng(element.latitude, element.longitude),
                map: map.instance,
                icon: image_hunter,
                // We store the document _id on the marker in order 
                // to update the document within the 'dragend' event below.
                id: document._id
                });
                          
                        
            google.maps.event.addListener(oneMarker, 'click', (function (event) 
            {
                var infowin;
                if (element.typeOfChallengeIndex == 0)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Quiz'
                    })
                }
                else if (element.typeOfChallengeIndex == 1)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Multiplayer'
                    })
                }
                else if (element.typeOfChallengeIndex == 2)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Hunter'
                    })
                }
                else if (element.typeOfChallengeIndex == 3)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Voting'
                    })
                }
                else if (element.typeOfChallengeIndex == 4)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timer Multiplayer (Ind.)'
                    })
                }
                else if (element.typeOfChallengeIndex == 5)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timed Task'
                    })
                }
                else if (element.typeOfChallengeIndex == 6)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Open Quiz'
                    })
                }
                else {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Other'
                    })
                }

                infoswindows.push(infowin);

                for(var i = 0; i < infoswindows.length; i++)
                {
                    infoswindows[i].close();
                }
                
                infowin.open(map, oneMarker);
            }));
        });


        /**
         *  Type of challenge: Voting Challenge
         */

        var image_voting = {
            url: '../images/votingicon.png',// image is in root directory of meteor project.
            size: new google.maps.Size(71, 71),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(35, 35)
            };
    
        listOfChallenges_voting.forEach(element => 
        {
            var oneMarker = new google.maps.Marker({
                draggable: false,
                animation: google.maps.Animation.DROP,
                position: new google.maps.LatLng(element.latitude, element.longitude),
                map: map.instance,
                icon: image_voting,
                // We store the document _id on the marker in order 
                // to update the document within the 'dragend' event below.
                id: document._id
                });
                    
                            
            google.maps.event.addListener(oneMarker, 'click', (function (event) 
            {
                var infowin;
                if (element.typeOfChallengeIndex == 0)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Quiz'
                    })
                }
                else if (element.typeOfChallengeIndex == 1)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Multiplayer'
                    })
                }
                else if (element.typeOfChallengeIndex == 2)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Hunter'
                    })
                }
                else if (element.typeOfChallengeIndex == 3)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Voting'
                    })
                }
                else if (element.typeOfChallengeIndex == 4)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timer Multiplayer (Ind.)'
                    })
                }
                else if (element.typeOfChallengeIndex == 5)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timed Task'
                    })
                }
                else if (element.typeOfChallengeIndex == 6)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Open Quiz'
                    })
                }
                //Challenges_Timed_TaskDB
                else {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Other'
                    })
                }

                infoswindows.push(infowin);

                for(var i = 0; i < infoswindows.length; i++)
                {
                    infoswindows[i].close();
                }
                
                infowin.open(map, oneMarker); 
            }));
        });

        /**
         * Type of challenges: Timer Multiplayer Independent
         */
        var image_timer_multiplayer_independent = {
            url: '../images/TimerMultiplayerIndependentIcon.png',// image is in root directory of meteor project.
            size: new google.maps.Size(71, 71),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(35, 35)
            };
    
        listOfChallenges_timer_multiplayer_independent.forEach(element => 
        {
            var oneMarker = new google.maps.Marker({
                draggable: false,
                animation: google.maps.Animation.DROP,
                position: new google.maps.LatLng(element.latitude, element.longitude),
                map: map.instance,
                icon: image_timer_multiplayer_independent,
                // We store the document _id on the marker in order 
                // to update the document within the 'dragend' event below.
                id: document._id
                });
                              
                            
            google.maps.event.addListener(oneMarker, 'click', (function (event) 
            {
                var infowin;
                if (element.typeOfChallengeIndex == 0)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Quiz'
                    })
                }
                else if (element.typeOfChallengeIndex == 1)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Multiplayer'
                    })
                }
                else if (element.typeOfChallengeIndex == 2)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Hunter'
                    })
                }
                else if (element.typeOfChallengeIndex == 3)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Voting'
                    })
                }
                else if (element.typeOfChallengeIndex == 4)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timer Multiplayer (Ind.)'
                    })
                }
                else if (element.typeOfChallengeIndex == 5)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timed Task'
                    })
                }
                else if (element.typeOfChallengeIndex == 6)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Open Quiz'
                    })
                }
                //Challenges_Timed_TaskDB
                else {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Other'
                    })
                }

                infoswindows.push(infowin);

                for(var i = 0; i < infoswindows.length; i++)
                {
                    infoswindows[i].close();
                }
                
                infowin.open(map, oneMarker);
            }));
        });


        /**
         * Type of challenges: Timed Task
         */
        var image_timedTask = {
            url: '../images/TimedTask.png',// image is in root directory of meteor project.
            size: new google.maps.Size(71, 71),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(35, 35)
            };
    
        listOfChallenges_timed_task.forEach(element => 
        {
            var oneMarker = new google.maps.Marker({
                draggable: false,
                animation: google.maps.Animation.DROP,
                position: new google.maps.LatLng(element.latitude, element.longitude),
                map: map.instance,
                icon: image_timedTask,
                // We store the document _id on the marker in order 
                // to update the document within the 'dragend' event below.
                id: document._id
                });
                              
                            
            google.maps.event.addListener(oneMarker, 'click', (function (event) 
            {
                var infowin;
                if (element.typeOfChallengeIndex == 0)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Quiz'
                    })
                }
                else if (element.typeOfChallengeIndex == 1)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Multiplayer'
                    })
                }
                else if (element.typeOfChallengeIndex == 2)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Hunter'
                    })
                }
                else if (element.typeOfChallengeIndex == 3)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Voting'
                    })
                }
                else if (element.typeOfChallengeIndex == 4)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timer Multiplayer (Ind.)'
                    })
                }
                else if (element.typeOfChallengeIndex == 5)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timed Task'
                    })
                }
                else if (element.typeOfChallengeIndex == 6)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Open Quiz'
                    })
                }
                //Challenges_Timed_TaskDB
                else {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Other'
                    })
                }

                infoswindows.push(infowin);

                for(var i = 0; i < infoswindows.length; i++)
                {
                    infoswindows[i].close();
                }
                
                infowin.open(map, oneMarker);
            }));
        });


        /**
         * Type of challenges: Timed Task
         */
        var image_OpenQuiz = {
            url: '../images/OpenQuiz.png',// image is in root directory of meteor project.
            size: new google.maps.Size(71, 71),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(35, 35)
            };
    
        listOfChallenges_openQuiz.forEach(element => 
        {
            var oneMarker = new google.maps.Marker({
                draggable: false,
                animation: google.maps.Animation.DROP,
                position: new google.maps.LatLng(element.latitude, element.longitude),
                map: map.instance,
                icon: image_OpenQuiz,
                // We store the document _id on the marker in order 
                // to update the document within the 'dragend' event below.
                id: document._id
                });
                              
                            
            google.maps.event.addListener(oneMarker, 'click', (function (event) 
            {
                var infowin;
                if (element.typeOfChallengeIndex == 0)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Quiz'
                    })
                }
                else if (element.typeOfChallengeIndex == 1)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Multiplayer'
                    })
                }
                else if (element.typeOfChallengeIndex == 2)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Hunter'
                    })
                }
                else if (element.typeOfChallengeIndex == 3)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Voting'
                    })
                }
                else if (element.typeOfChallengeIndex == 4)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timer Multiplayer (Ind.)'
                    })
                }
                else if (element.typeOfChallengeIndex == 5)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Timed Task'
                    })
                }
                else if (element.typeOfChallengeIndex == 6)
                {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Open Quiz'
                    })
                }
                //Challenges_Timed_TaskDB
                else {
                    infowin= new google.maps.InfoWindow({
                        content: 'Challenge: ' + element.name + '<br/>Type: Other'
                    })
                }

                infoswindows.push(infowin);

                for(var i = 0; i < infoswindows.length; i++)
                {
                    infoswindows[i].close();
                }
                
                infowin.open(map, oneMarker);
            }));
        });


        // more types of challenges
    });  
    
});





Template.Challenges.helpers({ 
    challenges() {
        var count = 0;
        listOfChallenges.forEach(element => 
            {
                count ++;
            });


        console.log("Number of challenges: " + count);
        return listOfChallenges;//ChallengesDB.find({});
      },
      challenges1() {
        
        var count = 0;
        listOfChallenges1.forEach(element => 
            {
                count ++;
            });


        console.log("Number of M. challenges: " + count);
        return listOfChallenges1;//ChallengesDB.find({});
      },
      challenges_hunter() {
        var count = 0;
        listOfChallenges_hunter.forEach(element => 
            {
                count ++;
            });


        console.log("Number of Hunter challenges: " + count);
        return listOfChallenges_hunter;//ChallengesDB.find({});
      },
      challenges_voting() {
        var count = 0;
        listOfChallenges_voting.forEach(element => 
            {
                count ++;
            });


        console.log("Number of Voting challenges: " + count);
        return listOfChallenges_voting;//ChallengesDB.find({});
      },
      challenges_timer_multiplayer_independent() {
        var count = 0;
        listOfChallenges_timer_multiplayer_independent.forEach(element => 
            {
                count ++;
            });


        console.log("Number of Timed Multiplayer independent challenges: " + count);
        return listOfChallenges_timer_multiplayer_independent;
      },
      challenges_timed_task() {
        var count = 0;
        listOfChallenges_timed_task.forEach(element => 
            {
                count ++;
            });


        console.log("Number of Timed Tasks: " + count);
        return listOfChallenges_timed_task;
      },
      challenges_openQuiz() {
        var count = 0;
        listOfChallenges_openQuiz.forEach(element => 
            {
                count ++;
            });


        console.log("Number of Open Quiz: " + count);
        return listOfChallenges_openQuiz;
      },
    introduceChallenge1() {
          return Template.instance().introduceChallenge1.get();
    },
    introduceChallengeHunter () {
        return Template.instance().introduceChallengeHunter.get();
    },
    introduceChallengeVoting () {
        return Template.instance().introduceChallengeVoting.get();
    },
    introduceChallengeTimerMultiplayerIndependent () {
        return Template.instance().introduceChallengeTimerMultiplayerIndependent.get();
    },
    introduceChallengeTimedTask () {
        return Template.instance().introduceChallengeTimedTask.get();
    },
    introduceChallengeOpenQuiz () {
        return Template.instance().introduceChallengeOpenQuiz.get();
    },
    nameOfCreatorOfChallenge() {
        return Meteor.user().username;
    },
    resetIntroduceChallenge1() {
        return Template.instance().introduceChallenge1.set(true);
    },
    resetIntroduceChallengeHunter () {
        return Template.instance().introduceChallengeHunter.set(true);
    },
    resetIntroduceChallengeVoting () {
        return Template.instance().introduceChallengeVoting.set(true);
    },
    resetIntroduceChallengeTimerMultiplayerIndependent () {
        return Template.instance().introduceChallengeTimerMultiplayerIndependent.set(true);
    },
    resetIntroduceChallengeTimedTask () {
        return Template.instance().introduceChallengeTimedTask.set(true);
    },
    resetIntroduceChallengeOpenQuiz () {
        return Template.instance().introduceChallengeOpenQuiz.set(true);
    },
    sharedReactiveParentVar() {
        //return this.sharedReactiveParentVar;
         return Template.instance().sharedReactiveParentVar.get();
    },
    resetSharedReactiveParentVar() {
        //return this.sharedReactiveParentVar;
        return Template.instance().sharedReactiveParentVar.set(true);
    },
    // porque estás a usar variáveis, precisas de as colocar no helper
    editMode: function () {
        return Template.instance().editMode.get();  // we are returning THIS SPECIFIC variable 
                                                    // from this template
    },
    showOverallMap: function () {
        return Template.instance().showOverallMap.get();
    },
    added_Location: function () {
        return Template.instance().added_Location.get();
    },
    newChallengeMode: function () {
        return Template.instance().newChallengeMode.get();
    },
    lat_addChallenge: function () {
        return Template.instance().lat_addChallenge.get();
    },
    lng_addChallenge: function () {
        return Template.instance().lng_addChallenge.get();
    },
    mapOptions: function() {
        if (GoogleMaps.loaded()) {
          return {
            center: new google.maps.LatLng(51.8944964, 4.5143659),
            zoom: 14
          };
        }
      },
      mapOptions2: function() {
          if (GoogleMaps.loaded()) {
            return {
              center: new google.maps.LatLng(51.8944964, 4.5143659),
              zoom: 11
            };
          }
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
          else if (index == 5)
          {
              return 'Timed Task';
          }else if (index == 6)
          {
              return 'Open Quiz';
          }
          else { return 'Other';}
      }
}); 


Template.Challenges.events ({
    
    'click .new-challenge': function() { 
        // famos fazer o set duma variável de sessão
        //Session.set('newRecipe', true);
        Template.instance().sharedReactiveParentVar.set(true);
        Template.instance().newChallengeMode.set(false);
        added_Location.set(false);
        
   },
   'click .new-challenge1': function() { 
    // famos fazer o set duma variável de sessão
    //Session.set('newRecipe', true);
    Template.instance().introduceChallenge1.set(true);
    Template.instance().newChallengeMode.set(false);
    added_Location.set(false);
    
},
'click .new-challenge-hunter': function() { 
    // famos fazer o set duma variável de sessão
    //Session.set('newRecipe', true);
    Template.instance().introduceChallengeHunter.set(true);
    Template.instance().newChallengeMode.set(false);
    added_Location.set(false);
    
},
'click .new-challenge-voting': function() { 
    // famos fazer o set duma variável de sessão
    //Session.set('newRecipe', true);
    Template.instance().introduceChallengeVoting.set(true);
    Template.instance().newChallengeMode.set(false);
    added_Location.set(false);
    
},
'click .new-challenge-timer-multiplayer-independent': function() { 
    // famos fazer o set duma variável de sessão
    //Session.set('newRecipe', true);
    Template.instance().introduceChallengeTimerMultiplayerIndependent.set(true);
    Template.instance().newChallengeMode.set(false);
    added_Location.set(false);
    
},
'click .new-challenge-timed-task': function() { 
    // famos fazer o set duma variável de sessão
    //Session.set('newRecipe', true);
    Template.instance().introduceChallengeTimedTask.set(true);
    Template.instance().newChallengeMode.set(false);
    added_Location.set(false);
    
},
'click .new-challenge-open-quiz': function() { 
    // famos fazer o set duma variável de sessão
    //Session.set('newRecipe', true);
    Template.instance().introduceChallengeOpenQuiz.set(true);
    Template.instance().newChallengeMode.set(false);
    added_Location.set(false);
    
},
'click .fa-trash1': function() { 
    Meteor.call('deleteChallenge1', this._id);
}, 
'click .fa-trash2': function() { 
    Meteor.call('deleteChallenge_Hunter', this._id);
},
'click .fa-trash3': function() { 
    Meteor.call('deleteChallenge_Voting', this._id);
}, 
'click .fa-trash4': function() { 
    Meteor.call('deleteChallenge_Timer_Multiplayer_Independent', this._id);
}, 
'click .fa-trash5': function() { 
    Meteor.call('deleteChallenge_Timed_Task', this._id);
}, 
'click .fa-trash6': function() { 
    Meteor.call('deleteChallenge_OpenQuiz', this._id);
}, 
   'click .fa-close': function() { 
        Template.instance().introduceChallenge1.set(false);
        Template.instance().introduceChallengeHunter.set(false);
        Template.instance().introduceChallengeVoting.set(false);
        Template.instance().introduceChallengeTimerMultiplayerIndependent.set(false);
        Template.instance().introduceChallengeTimedTask.set(false);
        Template.instance().introduceChallengeOpenQuiz.set(false);
        Template.instance().sharedReactiveParentVar.set(false);
        Template.instance().newChallengeMode.set(true);
   }/*, 
    'click .fa-pencil': function(event, template) { 
         template.editMode.set(!Template.instance().editMode.get());  // this is a reactiveVar, meaning a session 
                                                         // variable that is attached to a template of 
                                                         // a website
     } */, 

    'click .fa-trash': function() { 
        Meteor.call('deleteChallenge', this._id);
   }, 

    'submit .info-challenge-add'(event) {
        event.preventDefault();
        const target = event.target;
    
        const name = target.name;
        const description = target.description;
        const route = target.route;
        if ((route.value != 0) && (route.value != 1) && (route.value != 2) && (route.value != 3)) {route.value = 0;}
        
        const typeOfChallengeIndex = 0;
        
        const question = target.question;
        const answer = target.answer;
        const imageURL = target.imageURL;

        const ownerPlayFabID = Meteor.user().username;
        console.info('user id: ' + ownerPlayFabID);
        if (!!ownerPlayFabID)
        {
            if ((lat_addChallenge.get() == 0.0) || (lng_addChallenge.get() == 0.0))
            {
                
                alert('Please click on the map, in order to select the location of the challenge');
            }
            else
            {
                const latitude = lat_addChallenge.get();
                const longitude = lng_addChallenge.get();

                Meteor.call('challengesDB.insert', name.value, 
                description.value,
                ownerPlayFabID,
                typeOfChallengeIndex,
                latitude,
                longitude,
                question.value,
                answer.value,
                imageURL.value,
                route.value,
                false, (error) => {
                    if (error) {
                        alert(error.error);
                    } else {
                        
                        var text = 'A player of the SotS just inserted a new quiz challenge in the Secrets of the South webpage.';// Please validate the challenge.';
                        text = text.concat('\r\n\r\n\r\n********************* The Quiz Challenge *********************\r\nTitle: ');
                        text = text.concat(name.value);
                        text = text.concat('\r\nDescription: ');
                        text = text.concat(description.value);
                        text = text.concat('\r\nQuestion: ');
                        text = text.concat(question.value);
                        text = text.concat('\r\nAnswer: ');
                        text = text.concat(answer.value);
                        text = text.concat('\r\nImageURL: ');
                        text = text.concat(imageURL.value);
                        text = text.concat('\r\nOwner: ');
                        text = text.concat(ownerPlayFabID);
                        text = text.concat('\r\n*********************************************************');
                        text = text.concat('\r\n\r\n\r\nIn case this challenge complies with the game, you may consider validating it.\r\n\r\nBest regards,\r\nThe Secrets of the South');
                        Meteor.call('sendEmail', text, typeOfChallengeIndex);

                        name.value = ' '; 
                        description.value = ' ';
                        question.value = ' ';
                        answer.value = ' ';
                        imageURL.value = 'http://.../image.jpeg';
                        route.value = '0';
                        window.location.reload(true); 
                        alert("Quiz challenge introduced successfully :) Reloading...");
                    }
                });

                Template.instance().introduceChallengeTimedTask.set(false);
                Template.instance().introduceChallengeTimerMultiplayerIndependent.set(false);
                Template.instance().introduceChallengeVoting.set(false);
                Template.instance().introduceChallengeHunter.set(false);
                Template.instance().sharedReactiveParentVar.set(false);
                Template.instance().newChallengeMode.set(true);
                Template.instance().introduceChallengeOpenQuiz.set(false);

                lat_addChallenge.set(0.0); 
                lng_addChallenge.set(0.0);
                added_Location.set(false);
            }
        }
        else
        {
            FlowRouter.go('/');
        }
      },
      'submit .info-challenge1-add'(event) {
        event.preventDefault();
        const target = event.target;
    
        const name = target.name;
        const description = target.description;
        const route = target.route;
        if ((route.value != 0) && (route.value != 1) && (route.value != 2) && (route.value != 3)) {route.value = 0;}

        const typeOfChallengeIndex = 1; // Multiplayer
        
        const task = target.task;
        const imageURL = target.imageURL;

        const ownerPlayFabID = Meteor.user().username;
        console.info('user id: ' + ownerPlayFabID);
        if (!!ownerPlayFabID)
        {
            if ((lat_addChallenge.get() == 0.0) || (lng_addChallenge.get() == 0.0))
            {
                
                alert('Please click on the map, in order to select the location of the challenge');
            }
            else
            {
                const latitude = lat_addChallenge.get();
                const longitude = lng_addChallenge.get();


                Meteor.call('challenges1DB.insert', name.value, 
                description.value,
                ownerPlayFabID,
                typeOfChallengeIndex,
                latitude,
                longitude,
                task.value,
                imageURL.value,
                route.value,
                false, (error) => {
                    if (error) {
                        alert(error.error);
                    } else {
                        var text = 'A player of the SotS just inserted a new multiplayer challenge in the Secrets of the South webpage.';// Please validate the challenge.';
                        text = text.concat('\r\n\r\n\r\n********************* The Multiplayer Challenge *********************\r\nTitle: ');
                        text = text.concat(name.value);
                        text = text.concat('\r\nDescription: ');
                        text = text.concat(description.value);
                        text = text.concat('\r\nTask: ');
                        text = text.concat(task.value);
                        text = text.concat('\r\nImageURL: ');
                        text = text.concat(imageURL.value);
                        text = text.concat('\r\nOwner: ');
                        text = text.concat(ownerPlayFabID);
                        text = text.concat('\r\n*********************************************************');
                        text = text.concat('\r\n\r\n\r\nIn case this challenge complies with the game, you may consider validating it.\r\n\r\nBest regards,\r\nThe Secrets of the South');
                        Meteor.call('sendEmail', text, typeOfChallengeIndex);
                        
                        name.value = ' '; 
                        description.value = ' ';
                        task.value = ' ';
                        imageURL.value = 'http://.../image.jpeg';
                        route.value = '0';
                        window.location.reload(true); 
                        alert("Multiplayer challenge introduced successfully :) Reloading...");
                    }
                });

                Template.instance().introduceChallengeTimedTask.set(false);
                Template.instance().introduceChallengeTimerMultiplayerIndependent.set(false);
                Template.instance().introduceChallengeVoting.set(false);
                Template.instance().introduceChallengeHunter.set(false);
                Template.instance().introduceChallenge1.set(false);
                Template.instance().newChallengeMode.set(true);
                Template.instance().introduceChallengeOpenQuiz.set(false);

                lat_addChallenge.set(0.0); 
                lng_addChallenge.set(0.0);
                added_Location.set(false);
            }
        }
        else
        {
            FlowRouter.go('/');
        }
      },
      'submit .info-challengeHunter-add'(event) {
        event.preventDefault();
        const target = event.target;
    
        const name = target.name;
        const description = target.description;
        const route = target.route;
        if ((route.value != 0) && (route.value != 1) && (route.value != 2) && (route.value != 3)) {route.value = 0;}

        const typeOfChallengeIndex = 2; // Hunter
        
        const question = target.question;
        const answer = target.answer;
        const imageURL = target.imageURL;
        const content_text = target.content_text;
        const content_picture = target.content_picture;

        const ownerPlayFabID = Meteor.user().username;
        console.info('user id: ' + ownerPlayFabID);
        if (!!ownerPlayFabID)
        {
            if ((lat_addChallenge.get() == 0.0) || (lng_addChallenge.get() == 0.0))
            {
                
                alert('Please click on the map, in order to select the location of the challenge');
            }
            else if ( (content_picture.value === "") && (content_text.value === "") ) 
            {
                alert('You should provide the content to be shown to players within the QR code: either Text, or a link for a picture.');
            } 
            else
            {
                const latitude = lat_addChallenge.get();
                const longitude = lng_addChallenge.get();


                Meteor.call('Challenges_HunterDB.insert', name.value, 
                description.value,
                ownerPlayFabID,
                typeOfChallengeIndex,
                latitude,
                longitude,
                question.value,
                answer.value,
                imageURL.value,
                content_text.value,
                content_picture.value,
                route.value,
                false, (error) => {
                    if (error) {
                        alert(error.error);
                    } else {
                        var text = 'A player of the SotS just inserted a new hunter challenge in the Secrets of the South webpage.';// Please validate the challenge.';
                        text = text.concat('\r\n\r\n\r\n********************* The Hunter Challenge *********************\r\nTitle: ');
                        text = text.concat(name.value);
                        text = text.concat('\r\nDescription: ');
                        text = text.concat(description.value);
                        text = text.concat('\r\nQuestion: ');
                        text = text.concat(question.value);
                        text = text.concat('\r\nAnswer: ');
                        text = text.concat(answer.value);
                        text = text.concat('\r\nImageURL: ');
                        text = text.concat(imageURL.value);
                        text = text.concat('\r\nContent as Text: ');
                        text = text.concat(content_text.value);
                        text = text.concat('\r\nContent as Image: ');
                        text = text.concat(content_picture.value);
                        text = text.concat('\r\nOwner: ');
                        text = text.concat(ownerPlayFabID);
                        text = text.concat('\r\n*********************************************************');
                        text = text.concat('\r\n\r\n\r\nIn case this challenge complies with the game, you may consider validating it.\r\n\r\nBest regards,\r\nThe Secrets of the South');
                        Meteor.call('sendEmail', text, typeOfChallengeIndex);
                        
                        name.value = ' '; 
                        description.value = ' ';
                        question.value = ' ';
                        answer.value = ' ';
                        imageURL.value = 'http://.../image.jpeg';
                        content_text.value = ' ';
                        content_picture.value = ' ';
                        route.value='0';

                        window.location.reload(true); 
                        alert("Hunter challenge introduced successfully :) Reloading...");
                    }
                });

                Template.instance().introduceChallengeTimedTask.set(false);
                Template.instance().introduceChallengeTimerMultiplayerIndependent.set(false);
                Template.instance().introduceChallengeVoting.set(false);
                Template.instance().introduceChallengeHunter.set(false);
                Template.instance().introduceChallenge1.set(false);
                Template.instance().newChallengeMode.set(true);
                Template.instance().introduceChallengeOpenQuiz.set(false);

                lat_addChallenge.set(0.0); 
                lng_addChallenge.set(0.0);
                added_Location.set(false);
            }
        }
        else
        {
            FlowRouter.go('/');
        }
      },
      'submit .info-challengeVoting-add'(event) {
        event.preventDefault();
        const target = event.target;
    
        const name = target.name;
        const description = target.description;
        const route = target.route;
        if ((route.value != 0) && (route.value != 1) && (route.value != 2) && (route.value != 3)) {route.value = 0;}
        const typeOfChallengeIndex = 3; // Voting
        
        const task = target.task;
        const imageURL = target.imageURL;
        const listOfImagesAndVotes = "";
        

        const ownerPlayFabID = Meteor.user().username;
        console.info('user id: ' + ownerPlayFabID);
        if (!!ownerPlayFabID)
        {
            if ((lat_addChallenge.get() == 0.0) || (lng_addChallenge.get() == 0.0))
            {
                
                alert('Please click on the map, in order to select the location of the challenge');
            }
            else
            {
                const latitude = lat_addChallenge.get();
                const longitude = lng_addChallenge.get();


                Meteor.call('Challenges_VotingDB.insert', name.value, 
                description.value,
                ownerPlayFabID,
                typeOfChallengeIndex,
                latitude,
                longitude,
                task.value,
                imageURL.value,
                listOfImagesAndVotes.value,
                route.value,
                false, (error) => {
                    if (error) {
                        alert(error.error);
                    } else {
                        var text = 'A player of the SotS just inserted a new voting challenge in the Secrets of the South webpage.';// Please validate the challenge.';
                        text = text.concat('\r\n\r\n\r\n********************* The Voting Challenge *********************\r\nTitle: ');
                        text = text.concat(name.value);
                        text = text.concat('\r\nDescription: ');
                        text = text.concat(description.value);
                        text = text.concat('\r\nTask: ');
                        text = text.concat(task.value);
                        text = text.concat('\r\nImageURL: ');
                        text = text.concat(imageURL.value);
                        text = text.concat('\r\nOwner: ');
                        text = text.concat(ownerPlayFabID);
                        text = text.concat('\r\n*********************************************************');
                        text = text.concat('\r\n\r\n\r\nIn case this challenge complies with the game, you may consider validating it.\r\n\r\nBest regards,\r\nThe Secrets of the South');
                        Meteor.call('sendEmail', text, typeOfChallengeIndex);
                        
                        name.value = ' '; 
                        description.value = ' ';
                        task.value = ' ';
                        imageURL.value = 'http://.../image.jpeg';
                        route.value = '0';

                        window.location.reload(true); 
                        alert("Voting challenge introduced successfully :) Reloading...");
                    }
                });

                Template.instance().introduceChallengeTimedTask.set(false);
                Template.instance().introduceChallengeTimerMultiplayerIndependent.set(false);
                Template.instance().introduceChallengeVoting.set(false);
                Template.instance().introduceChallengeHunter.set(false);
                Template.instance().introduceChallenge1.set(false);
                Template.instance().newChallengeMode.set(true);
                Template.instance().introduceChallengeOpenQuiz.set(false);

                lat_addChallenge.set(0.0); 
                lng_addChallenge.set(0.0);
                added_Location.set(false);                
            }
        }
        else
        {
            FlowRouter.go('/');
        }
      },
      'submit .info-challengeTimerMultiplayerIndependent-add'(event) {
        event.preventDefault();
        const target = event.target;
        const name = target.name;
        const description = target.description;
        const typeOfChallengeIndex = 4; // Timer Multiplayer Independent
        const task = target.task;
        const imageURL = target.imageURL;
        const timer = target.timer;
        const ownerPlayFabID = Meteor.user().username; console.info('user id: ' + ownerPlayFabID);
        var timerPassed = parseInt(timer.value, 10);
        const route = target.route;
        if ((route.value != 0) && (route.value != 1) && (route.value != 2) && (route.value != 3)) {route.value = 0;}

        if (!!ownerPlayFabID)
        {
            if ((lat_addChallenge.get() == 0.0) || (lng_addChallenge.get() == 0.0))
            {
                alert('Please click on the map, in order to select the location of the challenge');
            }
            else if ((timerPassed < 1) || (timerPassed > 60))
            {
                alert('The amount of time for any task should be between 1 and 60 minutes (1 hour)');
            }
            else
            {
                const latitude = lat_addChallenge.get();
                const longitude = lng_addChallenge.get();

                Meteor.call('Challenges_Timer_Multiplayer_IndependentDB.insert', name.value, 
                description.value,
                ownerPlayFabID,
                typeOfChallengeIndex,
                latitude,
                longitude,
                task.value,
                imageURL.value,
                timerPassed,
                route.value,
                false, (error) => {
                    if (error) {
                        alert(error.error);
                    } else {
                        var text = 'A player of the SotS just inserted a new Timer Multiplayer (Ind.) challenge in the Secrets of the South webpage.';// Please validate the challenge.';
                        text = text.concat('\r\n\r\n\r\n********************* The Timer Multiplayer Independent Challenge *********************\r\nTitle: ');
                        text = text.concat(name.value);
                        text = text.concat('\r\nDescription: ');
                        text = text.concat(description.value);
                        text = text.concat('\r\nTask: ');
                        text = text.concat(task.value);
                        text = text.concat('\r\nImageURL: ');
                        text = text.concat(imageURL.value);
                        text = text.concat('\r\nTime Limit for the Task: ');
                        text = text.concat(timerPassed);
                        text = text.concat(' minutes');
                        text = text.concat('\r\nOwner: ');
                        text = text.concat(ownerPlayFabID);
                        text = text.concat('\r\n*********************************************************');
                        text = text.concat('\r\n\r\n\r\nIn case this challenge complies with the game, you may consider validating it.\r\n\r\nBest regards,\r\nThe Secrets of the South');
                        Meteor.call('sendEmail', text, typeOfChallengeIndex);
                        
                        name.value = ' '; 
                        description.value = ' ';
                        task.value = ' ';
                        imageURL.value = 'http://.../image.jpeg';
                        timer.value = '10';
                        route.value = '0';

                        window.location.reload(true); 
                        alert("Timer Multiplayer Independent challenge introduced successfully :) Reloading...");
                    }
                });

                Template.instance().introduceChallengeTimedTask.set(false);
                Template.instance().introduceChallengeTimerMultiplayerIndependent.set(false);
                Template.instance().introduceChallengeVoting.set(false);
                Template.instance().introduceChallengeHunter.set(false);
                Template.instance().introduceChallenge1.set(false);
                Template.instance().newChallengeMode.set(true);
                Template.instance().introduceChallengeOpenQuiz.set(false);

                lat_addChallenge.set(0.0); 
                lng_addChallenge.set(0.0);
                added_Location.set(false);                
            }
        }
        else
        {
            FlowRouter.go('/');
        }
    
      },
      'submit .info-challengeTimedTask-add'(event) {
        event.preventDefault();
        const target = event.target;
        const name = target.name;
        const description = target.description;
        const typeOfChallengeIndex = 5; // Timed Task
        const task = target.task;
        const imageURL = target.imageURL;
        const questionHowMany = target.questionHowMany;
        const timer = target.timer;
        var timerPassed = parseInt(timer.value, 10);
        const ownerPlayFabID = Meteor.user().username; console.info('user id: ' + ownerPlayFabID);
        const route = target.route;
        if ((route.value != 0) && (route.value != 1) && (route.value != 2) && (route.value != 3)) {route.value = 0;}

        if (!!ownerPlayFabID)
        {
            if ((lat_addChallenge.get() == 0.0) || (lng_addChallenge.get() == 0.0))
            {
                alert('Please click on the map, in order to select the location of the challenge');
            }
            else if ((timerPassed < 1) || (timerPassed > 60))
            {
                alert('The amount of time for any task should be between 1 and 60 minutes (1 hour)');
            }
            else
            {
                const latitude = lat_addChallenge.get();
                const longitude = lng_addChallenge.get();

                Meteor.call('Challenge_Timed_TaskDB.insert', name.value, 
                description.value,
                ownerPlayFabID,
                typeOfChallengeIndex,
                latitude,
                longitude,
                task.value,
                imageURL.value,
                questionHowMany.value,
                timerPassed,
                route.value,
                false, (error) => {
                    if (error) {
                        alert(error.error);
                    } else {
                        var text = 'A player of the SotS just inserted a new Timed Task challenge in the Secrets of the South webpage.';// Please validate the challenge.';
                        text = text.concat('\r\n\r\n\r\n********************* The Timed Task Challenge *********************\r\nTitle: ');
                        text = text.concat(name.value);
                        text = text.concat('\r\nDescription: ');
                        text = text.concat(description.value);
                        text = text.concat('\r\nTask: ');
                        text = text.concat(task.value);
                        text = text.concat('\r\nImageURL: ');
                        text = text.concat(imageURL.value);
                        text = text.concat('\r\nQuestion to ask after the timeout: ');
                        text = text.concat(questionHowMany.value);
                        text = text.concat('\r\nTime Limit for the Task: ');
                        text = text.concat(timerPassed);
                        text = text.concat(' minutes');
                        text = text.concat('\r\nOwner: ');
                        text = text.concat(ownerPlayFabID);
                        text = text.concat('\r\n*********************************************************');
                        text = text.concat('\r\n\r\n\r\nIn case this challenge complies with the game, you may consider validating it.\r\n\r\nBest regards,\r\nThe Secrets of the South');
                        Meteor.call('sendEmail', text, typeOfChallengeIndex);
                        
                        name.value = ' '; 
                        description.value = ' ';
                        task.value = ' ';
                        imageURL.value = 'http://.../image.jpeg';
                        questionHowMany.value = ' ';
                        timer.value = '10';
                        route.value = '0';

                        window.location.reload(true); 
                        alert("Timed task challenge introduced successfully :) Reloading...");
                    }
                });

                Template.instance().introduceChallengeTimedTask.set(false);
                Template.instance().introduceChallengeTimerMultiplayerIndependent.set(false);
                Template.instance().introduceChallengeVoting.set(false);
                Template.instance().introduceChallengeHunter.set(false);
                Template.instance().introduceChallenge1.set(false);
                Template.instance().newChallengeMode.set(true);
                Template.instance().introduceChallengeOpenQuiz.set(false);

                lat_addChallenge.set(0.0); 
                lng_addChallenge.set(0.0);
                added_Location.set(false);                
            }
        }
        else
        {
            FlowRouter.go('/');
        }
    
      },

      'submit .info-challengeOpenQuiz-add'(event) {
        event.preventDefault();
        const target = event.target;
        const name = target.name;
        const description = target.description;
        const typeOfChallengeIndex = 6; // Timed Task
        const question = target.question;
        const imageURL = target.imageURL;
        const route = target.route;
        if ((route.value != 0) && (route.value != 1) && (route.value != 2) && (route.value != 3)) {route.value = 0;}
        const ownerPlayFabID = Meteor.user().username; console.info('user id: ' + ownerPlayFabID);

        if (!!ownerPlayFabID)
        {
            if ((lat_addChallenge.get() == 0.0) || (lng_addChallenge.get() == 0.0))
            {
                alert('Please click on the map, in order to select the location of the challenge');
            }
            else
            {
                const latitude = lat_addChallenge.get();
                const longitude = lng_addChallenge.get();

                Meteor.call('Challenges_OpenQuizDB.insert', name.value, 
                description.value,
                ownerPlayFabID,
                typeOfChallengeIndex,
                latitude,
                longitude,
                question.value,
                imageURL.value,
                route.value,
                false, (error) => {
                    if (error) {
                        alert(error.error);
                    } else {
                        var text = 'A player of the SotS just inserted a new Open Quiz challenge in the Secrets of the South webpage.';// Please validate the challenge.';
                        text = text.concat('\r\n\r\n\r\n********************* The Timed Task Challenge *********************\r\nTitle: ');
                        text = text.concat(name.value);
                        text = text.concat('\r\nDescription: ');
                        text = text.concat(description.value);
                        text = text.concat('\r\Question: ');
                        text = text.concat(question.value);
                        text = text.concat('\r\nImageURL: ');
                        text = text.concat(imageURL.value);
                        text = text.concat('\r\nOwner: ');
                        text = text.concat(ownerPlayFabID);
                        text = text.concat('\r\n*********************************************************');
                        text = text.concat('\r\n\r\n\r\nIn case this challenge complies with the game, you may consider validating it.\r\n\r\nBest regards,\r\nThe Secrets of the South');
                        Meteor.call('sendEmail', text, typeOfChallengeIndex);
                        
                        name.value = ' '; 
                        description.value = ' ';
                        question.value = ' ';
                        imageURL.value = 'http://.../image.jpeg';
                        route.value = '0';
                        
                        window.location.reload(true); 
                        alert("Open Quiz challenge introduced successfully :) Reloading...");
                    }
                });

                Template.instance().introduceChallengeTimedTask.set(false);
                Template.instance().introduceChallengeTimerMultiplayerIndependent.set(false);
                Template.instance().introduceChallengeVoting.set(false);
                Template.instance().introduceChallengeHunter.set(false);
                Template.instance().introduceChallenge1.set(false);
                Template.instance().introduceChallengeOpenQuiz.set(false);
                Template.instance().newChallengeMode.set(true);

                lat_addChallenge.set(0.0); 
                lng_addChallenge.set(0.0);
                added_Location.set(false);                
            }
        }
        else
        {
            FlowRouter.go('/');
        }
    
      },
      'click .playfab_test': function() { 
        PlayFab.settings.titleId = document.getElementById("titleId").value;
        var loginRequest = {
            // Currently, you need to look up the correct format for this object in the API-docs:
            // https://api.playfab.com/documentation/Client/method/LoginWithCustomID
            TitleId: PlayFab.settings.titleId,
            CustomId: document.getElementById("customId").value,
            CreateAccount: true
        };
        
        PlayFabClientSDK.LoginWithCustomID(loginRequest, LoginCallback);
      }
   
});




var LoginCallback = function (result, error) {
    if (result !== null) {
        document.getElementById("resultOutput").innerHTML = "Congratulations, you made your first successful API call!";
    } else if (error !== null) {
        document.getElementById("resultOutput").innerHTML =
            "Something went wrong with your first API call.\n" +
            "Here's some debug information:\n" +
            PlayFab.GenerateErrorReport(error);
    }
}

