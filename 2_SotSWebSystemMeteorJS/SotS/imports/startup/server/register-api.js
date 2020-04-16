// Register your apis here
// https://github.com/kahmali/meteor-restivus

import '../../api/links/methods.js';
import '../../api/links/server/publications.js';
import { ChallengesDB } from '/Collections/Challenges.js';
import { Challenges1DB } from '/Collections/Challenges1.js';
import { Challenges_HunterDB } from '/Collections/Challenges_Hunter.js';
import { Challenges_VotingDB } from '/Collections/Challenges_Voting.js';
import { PlayerDataDB } from '/Collections/PlayerDataCollection.js';
import { PlayerLaskKnownLocationDB } from '/Collections/PlayerLastKnownLocationCollection.js';
import { TeamLastAccessedChallengeDB } from '/Collections/var_TeamLastAccessedChallenge.js';
import { GamePermissionsDB } from '/Collections/var_GamePermissions.js';
import { TeamsRankingDB } from '/Collections/var_TeamsRanking.js';
import { TeamsDB } from '/Collections/Teams.js';
import { Challenges_Timer_Multiplayer_IndependentDB } from '/Collections/Challenges_Timer_Multiplayer_Independent.js';
import { Challenges_Timed_TaskDB } from '/Collections/Challenges_Timed_Task.js';
import { Challenges_OpenQuizDB } from '/Collections/Challenges_OpenQuiz.js';


const JsonRoutes = require('meteor/simple:json-routes');
const Busboy = require('meteor/shammar13:busboy');

//PlayerDataDB
// PlayerLaskKnownLocationDB = new Mongo.Collection('PlayerLaskKnownLocation'); 

if (Meteor.isServer) {

    // Global API configuration
    /*var Api = new Restivus({
        useDefaultAuth: true,
        prettyJson: true
    });*/
    var Api = new Restivus({
        apiPath: 'api/',
        auth: {
          token: 'auth.apiKey',
          user: function () {
            return {
              userId: this.request.headers['user-id'],
              token: this.request.headers['login-token']
            };
          }
        },
        defaultHeaders: {
          'Content-Type': 'application/json'
        },
        onLoggedIn: function () {
          console.log(this.user.username + ' (' + this.userId + ') logged in');
        },
        onLoggedOut: function () {
          console.log(this.user.username + ' (' + this.userId + ') logged out');
        },
        prettyJson: true,
        useDefaultAuth: true
      });

      

    // Generates: GET, POST on /api/challenges and GET, PUT, PATCH, DELETE on
    // /api/challenges/:id for the ChallengesDB collection
    Api.addCollection(ChallengesDB, {
        excludedEndpoints: ['getAll'],
        routeOptions: {
          authRequired: false
        },
        endpoints: {
          post: {
            authRequired: false
          }
        }
      });

      Api.addCollection(Challenges1DB, {
        excludedEndpoints: ['getAll'],
        routeOptions: {
          authRequired: false
        },
        endpoints: {
          post: {
            authRequired: false
          }
        }
      });

      Api.addCollection(Challenges_HunterDB, {
        excludedEndpoints: ['getAll'],
        routeOptions: {
          authRequired: false
        },
        endpoints: {
          post: {
            authRequired: false
          }
        }
      });

      Api.addCollection(Challenges_VotingDB, {
        excludedEndpoints: ['getAll'],
        routeOptions: {
          authRequired: false
        },
        endpoints: {
          post: {
            authRequired: false
          }
        }
      });

      Api.addCollection(Challenges_Timer_Multiplayer_IndependentDB, {
        excludedEndpoints: ['getAll'],
        routeOptions: {
          authRequired: false
        },
        endpoints: {
          post: {
            authRequired: false
          }
        }
      });

      Api.addCollection(Challenges_Timed_TaskDB, {
        excludedEndpoints: ['getAll'],
        routeOptions: {
          authRequired: false
        },
        endpoints: {
          post: {
            authRequired: false
          }
        }
      });

      Api.addCollection(Challenges_OpenQuizDB, {
        excludedEndpoints: ['getAll'],
        routeOptions: {
          authRequired: false
        },
        endpoints: {
          post: {
            authRequired: false
          }
        }
      });

      // Generates: GET, POST on /api/challenges and GET, PUT, PATCH, DELETE on
      Api.addCollection(PlayerDataDB, {
        excludedEndpoints: ['getAll'],
        routeOptions: {
          authRequired: true
        },
        endpoints: {
          post: {
            authRequired: false
          }
        }
      });

      // Generates: GET, POST on /api/challenges and GET, PUT, PATCH, DELETE on
      Api.addCollection(TeamsRankingDB, {
        excludedEndpoints: ['getAll'],
        routeOptions: {
          authRequired: true
        },
        endpoints: {
          post: {
            authRequired: false
          }
        }
      });

      // Maps to: /api/playerdata?ownerPlayFabID=aesrdts&lat=52.0014568&lng=4.3280156&timestamp=asf2321&label=
    Api.addRoute('playerdata', {
        get: function () {
            var ownerPlayFabID = this.queryParams.ownerPlayFabID;
            var PlayerPlayFabID = ownerPlayFabID;
            var latitude = this.queryParams.lat;
            var longitude = this.queryParams.lng;
            var timestamp= this.queryParams.timestamp;
            var labelTmp = this.queryParams.label;
            var label; // = '# ' +  labelTmp.split("_").join(' ');

            if (labelTmp.length == 0)
            {
              label = ' ';
            }
            else 
            {
              label = '# ' +  labelTmp.split("_").join(' ');
            }

            if (ownerPlayFabID && latitude && longitude && timestamp && label)
            {
                  // store the last known location of this player in a separate table. Does the registry exist?
                  // if so, update. If not, insert.

                  var list = PlayerLaskKnownLocationDB.find();
                  var found = false;
                  var thisID;

                  list.forEach(element => 
                    {
                      console.log("Element:  " + element.PlayerPlayFabID);
                        if (new String(element.PlayerPlayFabID).valueOf() == new String(PlayerPlayFabID).valueOf())
                        {
                          console.log("Found the element ID" + element._id + " for the Player " + PlayerPlayFabID);
                          found = true;

                          thisID = element._id;
                          Meteor.call('UpdatePlayerLaskKnownLocation', thisID, 
                          PlayerPlayFabID, latitude, longitude, timestamp, 
                            (error) => {
                                if (error) {
                                    //return "Could not update player last location";
                                    console.log("Could not update player last location for player: " + PlayerPlayFabID);
                                } else {
                                  console.log("Location updated for player: " + PlayerPlayFabID);
                                  
                                }
                            });
                        }
                    });

                    // if you do not find, then add
                    if (!found)
                    {
                      console.log("Did not find the Player " + PlayerPlayFabID +". Adding it for the first time.");

                      PlayerLaskKnownLocationDB.insert({PlayerPlayFabID, latitude, longitude, timestamp});

                    }
                    
                    return PlayerDataDB.insert({
                      ownerPlayFabID, 
                      latitude, longitude,timestamp, label});
                  
                }
                else
                {
                    return false;
                }
            }
    });

    function UpdateListOfVotingsInFolderForVotingChallenge(path, challengeid) {
      var list = Challenges_VotingDB.find({_id:challengeid});
      var fs     = Npm.require("fs");

      var tmpPlayFabID;
      var images;
      var updatedListOfVotings = [];
      list.forEach(element => 
      { 
          // now, for each vote recorded in the DB, double check that there is a file. If not, remove
          var listOfVotings = element.listOfImagesAndVotes;
          if (listOfVotings) {
              for (var i = 0; i < listOfVotings.length; i++) {
                  // per voting done, grab the playfabid, which is the prefix of the files in the server
                  tmpPlayFabID = listOfVotings[i].split('_')[0];  
                  images = fs.readdirSync(path).filter(
                    function(i) {
                      return i.includes(tmpPlayFabID);
                    }
                  );

                  // if you got images, then keep the voting. Otherwise, clean the entrange
                  if ((!images) || ( images.length <= 0)) {
                      // Clean the entrange in the voting list
                      // which, I think in this case can be done by doing nothing.
                  }
                  else {
                    updatedListOfVotings.push(listOfVotings[i]);
                  }
              }

              // In here, I can commit the change done to the listOfVotings
              Meteor.call('updateChallenge_Voting', element._id, element.name, element.description, 
              element.ownerPlayFabID, element.typeOfChallengeIndex, element.latitude, element.longitude,
              element.task, element.imageURL, updatedListOfVotings, element.route, element.validated,
                  (error) => {
                      if (error) {
                          //return "Could not update player last location";
                          console.log("Could not update the List<string> of votings for this challenge: " + error);
                          succeeded = false;
                          return false;
                      } else {
                        console.log("Voting Challenge with ID " + challengeid + " updated Successfully.");
                        succeeded = true;
                        return true;
                      }
                  });
          }
          else {return true;}
      });

      return true;    
    }

    function UpdateListOfVotingsInFolderForVotingChallengeObject(path, challengeid, element) {
      //var list = Challenges_VotingDB.find({_id:challengeid});
      var fs     = Npm.require("fs");

      var tmpPlayFabID;
      var images;
      var updatedListOfVotings = [];
      //list.forEach(element => 
      // now, for each vote recorded in the DB, double check that there is a file. If not, remove
      var listOfVotings = element.listOfImagesAndVotes;
      if (listOfVotings) {
          for (var i = 0; i < listOfVotings.length; i++) {
              // per voting done, grab the playfabid, which is the prefix of the files in the server
              tmpPlayFabID = listOfVotings[i].split('_')[0];  
              images = fs.readdirSync(path).filter(
                function(i) {
                  return i.includes(tmpPlayFabID);
                }
              );

              // if you got images, then keep the voting. Otherwise, clean the entrange
              if ((!images) || ( images.length <= 0)) {
                  // Clean the entrange in the voting list
                  // which, I think in this case can be done by doing nothing.
              }
              else {
                updatedListOfVotings.push(listOfVotings[i]);
              }
          }

          // In here, I can commit the change done to the listOfVotings
          Meteor.call('updateChallenge_Voting', element._id, element.name, element.description, 
          element.ownerPlayFabID, element.typeOfChallengeIndex, element.latitude, element.longitude,
          element.task, element.imageURL, updatedListOfVotings, element.route, element.validated,
              (error) => {
                  if (error) {
                      //return "Could not update player last location";
                      console.log("Could not update the List<string> of votings for this challenge: " + error);
                      succeeded = false;
                      return false;
                  } else {
                    console.log("Voting Challenge with ID " + challengeid + " updated Successfully.");
                    succeeded = true;
                    return true;
                  }
              });
      }
      else {return true;}

      return true;    
    }

    // Maps to: /api/voteonvotingchallenge?challengeid=CMCY8NAxEDRigaJ2f&playfabID=F9C1620AF8977C96&vote=2
    Api.addRoute('voteonvotingchallenge', {
      get: function () { 
        var challengeid = this.queryParams.challengeid;
        var vote = this.queryParams.vote; // vote will be 0, 1, 2, 3, 4, or 5
        var playfabID = this.queryParams.playfabID;
        var found = false;
        //console.log("Entered 0" );
        // if you passed all the parameters
        if ((challengeid) && (vote) && (playfabID)) {
          //console.log("Entered 1" );
          // first, search the identified file inside the challenge folder, to record its voting
          var fs     = Npm.require("fs");
          var path;
          var succeeded = true;
          if (process.env.NODE_ENV === "development") {
            path = process.env["PWD"] + "/public/VotingChallenges/" + challengeid + "/";
          }
          else {
              try {
                  if (!fs.existsSync(process.env["PWD"] + "/programs/web.browser/app/VotingChallenges/")) {
                    // If folder VotingChallenges does not exist, then set it up
                    fs.mkdir(process.env["PWD"] + "/programs/web.browser/app/VotingChallenges/", { recursive: false }, (err) => {
                        if (err) {
                          succeeded = false;// throw err; 
                          errMessage = err;
                          console.log("Error creating VotingChallenges folder: " + err);
                        }
                      });
                }
              }
              catch (err) {
                succeeded = false;
                errMessage = err;
                console.log("Error: " + err);
              }

              path = process.env["PWD"] + "/programs/web.browser/app/VotingChallenges/"+ challengeid + "/";
          }

          //console.log("Entered 2" );

          if (succeeded) {
              // you found the folder, now try to find the image being voted for, in the system
              try { 
                //console.log("Entered 3" );
                // verify if folder exists
                  if (fs.existsSync(path)) {
                      var images = fs.readdirSync(path).filter(
                        function(i) {
                          console.log('File in directory: ', i);
                          return i.includes(playfabID);
                        }
                      );
                      //console.log("Entered 4" );
                      console.log('Number of occurences in Folder: ', images.length);
                      // if you found the image being voted for, then proceed 
                      if (images.length > 0)
                      {
                        //console.log("Entered 5" );
                          // now I should be working a bit of magic in the List<string> stored in the DB.
                          // the idea is to search for existent votings done for that picture. if
                          // not found, then introduce the new voting. If found, then add it
                          var list = Challenges_VotingDB.find({_id:challengeid});
                          list.forEach(element => 
                          {
                              if (new String(element._id).valueOf() == new String(challengeid).valueOf())
                              {
                                  console.log("Found the Voting Challenge in the server: " + element._id);
                                  found = true;
                                  var listOfVotings = element.listOfImagesAndVotes;
                                  if (listOfVotings) {
                                    // in this case, we already have votings for pictures in this 
                                    // challenge. Work them out before changing the list of votings
                                      var index = -1;
                                      for (var i = 0; i < listOfVotings.length; i++) {
                                            // now, we need to search for any voting our picture might have.
                                            if (new String(listOfVotings[i].split('_')[0]).valueOf() == new String(playfabID).valueOf()) {
                                              index = i;
                                              break;
                                            }
                                      }

                                      // Either we find a registry on that, or not. If we do, we need
                                      // to parse it and add to it. If not, we need to create a new
                                      // entry on the array with the brand new voting.
                                      if (index != -1) {
                                          // we found an entry. Update it
                                          //     0      1 2 3 4 5 6 7 8 9 10111213
                                          // playfabid_40_0_1_1_0_2_0_3_0_4_0_5_0
                                          var voteData = listOfVotings[index].split('_');
                                          var strToPush = ""+playfabID + "_" + (parseInt(voteData[1]) + 1) + "_";
                                          switch (parseInt(vote)) {
                                            case 0: 
                                                    strToPush += "0_" + (parseInt(voteData[3]) + 1) + "_1_" +
                                                    voteData[5] + "_2_" + voteData[7] + "_3_" + voteData[9]+"_4_" + voteData[11] + "_5_"+
                                                    voteData[13];
                                                    break;
                                            case 1: 
                                                    strToPush += "0_" + voteData[3] + "_1_" +
                                                    (parseInt(voteData[5]) + 1) + "_2_" + voteData[7] + "_3_" + voteData[9]+"_4_" + voteData[11] + "_5_"+
                                                    voteData[13];
                                                    break;
                                            case 2: 
                                                    strToPush += "0_" + voteData[3] + "_1_" +
                                                    voteData[5] + "_2_" + (parseInt(voteData[7]) +1) + "_3_" + voteData[9]+"_4_" + voteData[11] + "_5_"+
                                                    voteData[13];
                                                    break;
                                            case 3: 
                                                    strToPush += "0_" + voteData[3] + "_1_" + voteData[5] + "_2_" + voteData[7] +
                                                    "_3_" + (parseInt(voteData[9]) + 1)+"_4_" + voteData[11] + "_5_"+voteData[13];
                                                    break;
                                            case 4: 
                                                    strToPush += "0_" + voteData[3] + "_1_" + voteData[5] + "_2_" + voteData[7] +
                                                    "_3_" + voteData[9]+"_4_" + (parseInt(voteData[11]) + 1) + "_5_"+voteData[13];
                                                    break;
                                            case 5: 
                                                    strToPush += "0_" + voteData[3] + "_1_" + voteData[5] + "_2_" + voteData[7] +
                                                    "_3_" + voteData[9]+"_4_" + voteData[11] + "_5_"+(parseInt(voteData[13]) + 1);
                                                    break;
                                            default:
                                                    console.log('The vote given is invalid: ', vote,'. Not voting.');
                                                    succeeded = false;
                                                    return false;
                                          }

                                          var tmpArray= [];
                                          for (var j = 0; j < listOfVotings.length; j++) {
                                              // now, we need to search for any voting our picture might have.
                                              if (j != index) {
                                                tmpArray.push(listOfVotings[j]);
                                              }
                                          }
                                          tmpArray.push (strToPush);
                                          listOfVotings = tmpArray;

                                      }else {
                                          // no entry found. Create new
                                          var strToPush = ""+playfabID + "_1_";
                                          switch (parseInt(vote)) {
                                            case 0: 
                                                    strToPush += "0_1_1_0_2_0_3_0_4_0_5_0";
                                                    break;
                                            case 1: 
                                                    strToPush += "0_0_1_1_2_0_3_0_4_0_5_0";
                                                    break;
                                            case 2: 
                                                    strToPush += "0_0_1_0_2_1_3_0_4_0_5_0";
                                                    break;
                                            case 3: 
                                                    strToPush += "0_0_1_0_2_0_3_1_4_0_5_0";
                                                    break;
                                            case 4: 
                                                    strToPush += "0_0_1_0_2_0_3_0_4_1_5_0";
                                                    break;
                                            case 5: 
                                                    strToPush += "0_0_1_0_2_0_3_0_4_0_5_1";
                                                    break;
                                            default:
                                                    console.log('The vote given is invalid: ', vote,'. Not voting.');
                                                    succeeded = false;
                                                    return false;
                                          }
                                          
                                          listOfVotings.push(strToPush);
                                      }



                                  } else {
                                    // then, in this case, the registry in the DB is null. Introduce this
                                    // voting alone and commit
                                    var tmp = [];
                                    var strToPush = ""+playfabID + "_1_";

                                    switch (parseInt(vote)) {
                                      case 0: 
                                              strToPush += "0_1_1_0_2_0_3_0_4_0_5_0";
                                              break;
                                      case 1: 
                                              strToPush += "0_0_1_1_2_0_3_0_4_0_5_0";
                                              break;
                                      case 2: 
                                              strToPush += "0_0_1_0_2_1_3_0_4_0_5_0";
                                              break;
                                      case 3: 
                                              strToPush += "0_0_1_0_2_0_3_1_4_0_5_0";
                                              break;
                                      case 4: 
                                              strToPush += "0_0_1_0_2_0_3_0_4_1_5_0";
                                              break;
                                      case 5: 
                                              strToPush += "0_0_1_0_2_0_3_0_4_0_5_1";
                                              break;
                                      default:
                                              console.log('The vote given is invalid: ', vote,'. Not voting.');
                                              succeeded = false;
                                              return false;
                                    }

                                    tmp.push(strToPush);
                                    listOfVotings = tmp;
                                  }
                                  
                                  // In here, I can commit the change done to the listOfVotings
                                  Meteor.call('updateChallenge_Voting', element._id, element.name, element.description, 
                                  element.ownerPlayFabID, element.typeOfChallengeIndex, element.latitude, element.longitude,
                                  element.task, element.imageURL, listOfVotings, element.route, element.validated,
                                      (error) => {
                                          if (error) {
                                              //return "Could not update player last location";
                                              console.log("Could not update the List<string> of votings for this challenge: " + error);
                                              succeeded = false;
                                              return false;
                                          } else {
                                            console.log("Voting Challenge with ID " + challengeid + " updated Successfully.");
                                            succeeded = true;
                                            return true;
                                          }
                                      });

                                  // additionally, you might want to update the list of votings in this challenge,
                                  // as you might not have all the pictures in the server anymore
                                  // ...
                                  // succeeded = true;
                                  // return succeeded;

                              }                                
                          });
                          //console.log("Arrived here");
                          return succeeded;
                      }
                      else {
                        console.log('Did not find the picture being voted for, in the server');
                        // Update list of votings in DB. Keeps only the votings with a picture in folder
                        UpdateListOfVotingsInFolderForVotingChallenge(path, challengeid);

                        return false;
                      }
                  }
                  else {
                    console.log("Error. Path Does not exist:" + path);
                    return false;
                  }
              }
              catch(err) {
                succeeded = false;
                errMessage = err;
                console.log("Error: " + err);
            }
          } else {
            // console.log("Succeeded is false");
            return false;
          }
        }
        else {
          // if you did not pass all the parameters, then, fail
          //return found;
          console.log("You did not pass all the parameters?");
          return false;
        }        
      }
    });

  // Maps to: /api/teams?PlayerPlayFabID=aesrdts&TeamID=1294q4etfq&TeamName=Amazing_team&TeamRefIcon=TeamIcons/21
  Api.addRoute('teams', {
    get: function () {
        var PlayerPlayFabID = this.queryParams.PlayerPlayFabID;
        var TeamID = this.queryParams.TeamID;
        var TeamNameTmp = this.queryParams.TeamName;
        var TeamRefIcon= this.queryParams.TeamRefIcon;
        var TeamName = TeamNameTmp.split("_").join(' ');

        
        if (PlayerPlayFabID && TeamID && TeamName && TeamRefIcon)
        {
              // store the last known location of this player in a separate table. Does the registry exist?
              // if so, update. If not, insert.

              var list = TeamsDB.find();
              var found = false;
              var thisID;

              list.forEach(element => 
                {
                  console.log("Element:  " + element.PlayerPlayFabID);
                    if (new String(element.PlayerPlayFabID).valueOf() == new String(PlayerPlayFabID).valueOf())
                    {
                      console.log("Found the element ID" + element._id + " for the Player " + PlayerPlayFabID);
                      found = true;

                      thisID = element._id;
                      // this_PlayerPlayFabID, this_TeamID, this_TeamName, this_TeamRefIcon
                      Meteor.call('UpdatePlayerTeam', thisID, 
                      PlayerPlayFabID, TeamID, TeamName, TeamRefIcon, 
                        (error) => {
                            if (error) {
                                //return "Could not update player last location";
                                console.log("Could not update this player's most recent team: " + PlayerPlayFabID);
                            } else {
                              console.log("Team updated for player: " + PlayerPlayFabID);
                              
                            }
                        });
                    }
                });

                var ret = true;
                // if you do not find, then add
                if (!found)
                {
                  console.log("Did not find the Player " + PlayerPlayFabID +". Adding it for the first time.");

                  ret = TeamsDB.insert({PlayerPlayFabID, TeamID, TeamName, TeamRefIcon});

                }
                
                return ret;
              
            }
            else
            {
                return false;
            }
        }
});


// Maps to: /api/entermultiplayerChallenge?TeamID=201805222013493&ChallengeID=cLthJn43m3r2jGiP7
Api.addRoute('entermultiplayerChallenge', {
  get: function () {
      var this_TeamID = this.queryParams.TeamID;
      var this_challengeID = this.queryParams.ChallengeID;
      
      if (this_TeamID && this_challengeID)
      {
            
        var listOfPlayersInTeam = TeamsDB.find({TeamID:this_TeamID});
        var multiplayerChallenge = Challenges1DB.find({_id:this_challengeID});
        var listOfLastSeenChallengesByThisTeam = TeamLastAccessedChallengeDB.find({TeamID:this_TeamID});
        var playerLastknownLocation;
        var challengeLat;
        var challengeLng;



        multiplayerChallenge.forEach(element => 
          {
            console.log("Element:  " + element.PlayerPlayFabID);
              if (new String(element._id).valueOf() == new String(this_challengeID).valueOf())
              {
                console.log("Found the element ID" + element._id);
                
                challengeLat = element.latitude;
                challengeLng = element.longitude;
              }
          });
          console.log("Challenge coordinates: " + challengeLat + "  -  " + challengeLng);

          var circumference = 40075.0; // Earth's circumference at the equator in km
          var distance = 0.0;
          var latitude1Rad, longitude1Rad, latititude2Rad, longitude2Rad, logitudeDiff, angleCalculation;

          var playersLats = [];
          var playersLngs = [];
          //cars.push('1');

          listOfPlayersInTeam.forEach(teamElement => 
            {
              // get last known location of each player player
              playerLastknownLocation = PlayerLaskKnownLocationDB.find({PlayerPlayFabID:teamElement.PlayerPlayFabID});
              playerLastknownLocation.forEach(player => 
                {
                  playersLats.push(player.latitude);
                  playersLngs.push(player.longitude);
                  // i am ignoring the timestamp for now
                });

            });

            console.log("I have coordinates for " + playersLats.length + " players in this team.");
            var atLeastOnePlayerIsNotPresent = false;

            for (var i = 0; i < playersLats.length; i++)
            {
                latitude1Rad = challengeLat * Math.PI / 180.0;
                longitude1Rad = challengeLng * Math.PI / 180.0;
                latititude2Rad = playersLats[i] * Math.PI / 180.0;
                longitude2Rad = playersLngs[i] * Math.PI / 180.0;


                logitudeDiff = Math.abs(longitude1Rad - longitude2Rad);
                if (logitudeDiff > Math.PI)
                {
                    logitudeDiff = 2.0 * Math.PI - logitudeDiff;
                }
                angleCalculation =
                Math.acos(
                    Math.sin(latititude2Rad) * Math.sin(latitude1Rad) +
                    Math.cos(latititude2Rad) * Math.cos(latitude1Rad) * 
                    Math.cos(logitudeDiff));

                distance = circumference * angleCalculation / (2.0 * Math.PI);

                if (distance >= 30) // 30 meters
                {
                  atLeastOnePlayerIsNotPresent = true;
                  break;
                }
            }
          

            if (atLeastOnePlayerIsNotPresent) // 30 meters
              {
                return false;
              }
              else {

                // if it is to allow the team to enter, then store this info as the last challenge that the team saw, for the evaluators to rate it

                  var found = false;
                  var thisID;
                  var TeamID = this_TeamID, LastChallengeIDSeen = this_challengeID;

                  listOfLastSeenChallengesByThisTeam.forEach(element => 
                    {
                      //console.log("This team already saw this challenge:  " + element.LastChallengeIDSeen);
                        if (new String(element.TeamID).valueOf() == new String(TeamID).valueOf())
                        {
                          console.log("This team " + element.TeamID + " already recorded a challenge before: " + element.LastChallengeIDSeen);
                          found = true;

                          thisID = element._id;
                          Meteor.call('TeamLastAccessedChallengeDB', thisID, 
                          TeamID, LastChallengeIDSeen, 
                            (error) => {
                                if (error) {
                                    //return "Could not update player last location";
                                    console.log("Could not update the " +this_TeamID+" team's last visited challenge " + this_challengeID);
                                } else {
                                    console.log(this_TeamID+" team updated last visited challenge " + this_challengeID);
                                  
                                }
                            });
                        }
                    });

                    // if you do not find, then add
                    if (!found)
                    {
                      console.log("Did not find any challenge in the given team. Adding one for the first time.");
                      
                      TeamLastAccessedChallengeDB.insert({TeamID, LastChallengeIDSeen});

                    }

                return true;
              }

        }
        else
        {
            return false;
        }
    }
});

// Maps to: /api/numberofplayersinteam?TeamID=201805222013493
Api.addRoute('numberofplayersinteam', {
  get: function () {
      var this_TeamID = this.queryParams.TeamID;
      
      if (this_TeamID )
      {
            
        var listOfPlayersInTeam = TeamsDB.find({TeamID:this_TeamID});
        var players = [];


          listOfPlayersInTeam.forEach(element => 
          {
            players.push(element.PlayerPlayFabID);
          });
          
          return players;

        }
        else
        {
            return 0;
        }
    }
});




    // ChallengesDB
    // Maps to: /api/challenges_quiz?id=
  Api.addRoute('challenges_quiz', {
        get: function () {
          var myID = this.queryParams.id;
          var obj =   ChallengesDB.findOne(myID);
          if (obj)
            {return obj;}
          else 
            {return "";}
        }
    });

    // ChallengesDB
    // Maps to: /api/challenges_multiplayer?id=
  Api.addRoute('challenges_multiplayer', {
      get: function () {
        var myID = this.queryParams.id;
        var obj =  Challenges1DB.findOne(myID);
        if (obj)
        {return obj;}
        else {return "";}
      }
  });

// Challenges_HunterDB
    // Maps to: /api/challenges_hunter?id=mbysampX5jpTmJKEi
    Api.addRoute('challenges_hunter', {
      get: function () {
        var myID = this.queryParams.id;
        var obj = Challenges_HunterDB.findOne(myID);
        if (obj)
        {return obj;}
        else {return "";}
        
      }
  });

  // Challenges_VotingDB
    // Maps to: /api/challenges_voting?id=mbysampX5jpTmJKEi
    Api.addRoute('challenges_voting', {
      get: function () {
        var myID = this.queryParams.id;
        var obj = Challenges_VotingDB.findOne(myID);
        var path;
        if (obj)
        {   
            if (process.env.NODE_ENV === "development") {
              path = process.env["PWD"] + "/public/VotingChallenges/" + myID + "/";
            }
            else {
                try {
                    if (!fs.existsSync(process.env["PWD"] + "/programs/web.browser/app/VotingChallenges/")) {
                      // If folder VotingChallenges does not exist, then set it up
                      fs.mkdir(process.env["PWD"] + "/programs/web.browser/app/VotingChallenges/", { recursive: false }, (err) => {
                          if (err) {
                            succeeded = false;// throw err; 
                            errMessage = err;
                            console.log("Error: " + err);
                          }
                        });
                  }
                }
                catch (err) {
                  succeeded = false;
                  errMessage = err;
                  console.log("Error: " + err);
                }

                path = process.env["PWD"] + "/programs/web.browser/app/VotingChallenges/"+ myID + "/";
            }

            // in here, verify that the files of specified votings exist. If not, delete entry in voting
            UpdateListOfVotingsInFolderForVotingChallengeObject(path, myID,obj);

            // now that you potentially changed the object, you need to get it again
            var obj = Challenges_VotingDB.findOne(myID);
            if (obj) {
              return obj;
            }
            else {
              return "";
            }
            
        }
        else {return "";}
        
      }
  });

  // Challenges_Timer_Multiplayer_IndependentDB
  // Maps to: /api/challenges_timer_multiplayer_independent?id=mbysa...mJKEi
  Api.addRoute('challenges_timer_multiplayer_independent', {
    get: function () {
      var myID = this.queryParams.id;
        var obj = Challenges_Timer_Multiplayer_IndependentDB.findOne(myID);
        if (obj)
        {return obj;}
        else {return "";}
      
    }
  });


  // Challenges_Timed_TaskDB
  // Maps to: /api/challenges_timed_task?id=mbysa...mJKEi
  Api.addRoute('challenges_timed_task', {
    get: function () {
      var myID = this.queryParams.id;
        var obj = Challenges_Timed_TaskDB.findOne(myID);
        if (obj)
        {return obj;}
        else {return "";}
      
    }
  });

  // ChallengesDB
    // Maps to: /api/challenges_open_quiz?id=
    Api.addRoute('challenges_open_quiz', {
      get: function () {
        var myID = this.queryParams.id;
        var obj =   Challenges_OpenQuizDB.findOne(myID);
        if (obj)
          {return obj;}
        else 
          {return "";}
      }
  });

// Maps to: /api/challengesnearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
Api.addRoute('challengesnearby', {
    get: function () {
      var maxDistanceFromPlayer = this.queryParams.maxDistanceFromPlayer;
      var playerLat = this.queryParams.playerLat;
      var playerLng = this.queryParams.playerLng;
      //console.log("maxDistanceFromPlayer: " + maxDistanceFromPlayer); // result: 1234
      //console.log("playerLat: " + playerLat); // result: 10
      //console.log("playerLng: " + playerLng); // result: 10


      var circumference = 40075.0; // Earth's circumference at the equator in km
      var distance = 0.0;
      var latitude1Rad, longitude1Rad, latititude2Rad, longitude2Rad, logitudeDiff, angleCalculation;

      var listOfChallenges = ChallengesDB.find({}); 
      var listOfCloseChallenges = [];

      listOfChallenges.forEach(challenge => 
        { 
            latitude1Rad = challenge.latitude * Math.PI / 180.0;
            longitude1Rad = challenge.longitude * Math.PI / 180.0;
            latititude2Rad = playerLat * Math.PI / 180.0;
            longitude2Rad = playerLng * Math.PI / 180.0;


            //Calculate radians Math.abs()
            logitudeDiff = Math.abs(longitude1Rad - longitude2Rad);
            if (logitudeDiff > Math.PI)
            {
                logitudeDiff = 2.0 * Math.PI - logitudeDiff;
            }
            angleCalculation =
            Math.acos(
                Math.sin(latititude2Rad) * Math.sin(latitude1Rad) +
                Math.cos(latititude2Rad) * Math.cos(latitude1Rad) * 
                Math.cos(logitudeDiff));

            distance = circumference * angleCalculation / (2.0 * Math.PI);

            if ((distance <= maxDistanceFromPlayer) && (challenge.validated))
            {
                //{console.log("Challenge " +challenge._id + " is validated at " + distance + " from player and close enough. Add it!");}
                listOfCloseChallenges.push(challenge);
            }
            /*else {
                if (challenge.validated)
                {console.log("Challenge " +challenge._id + " is validated at " + distance + " from player.");}
                else 
                {console.log("Challenge " +challenge._id + " is not validated at " + distance + " from player.");}
            }*/
        });

      return listOfCloseChallenges;
    }
  });

    

  // Maps to: /api/challenges1nearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
Api.addRoute('challenges1nearby', {
  get: function () {
    var maxDistanceFromPlayer = this.queryParams.maxDistanceFromPlayer;
    var playerLat = this.queryParams.playerLat;
    var playerLng = this.queryParams.playerLng;
    //console.log("maxDistanceFromPlayer: " + maxDistanceFromPlayer); // result: 1234
    //console.log("playerLat: " + playerLat); // result: 10
    //console.log("playerLng: " + playerLng); // result: 10


    var circumference = 40075.0; // Earth's circumference at the equator in km
    var distance = 0.0;
    var latitude1Rad, longitude1Rad, latititude2Rad, longitude2Rad, logitudeDiff, angleCalculation;

    var listOfChallenges = Challenges1DB.find({}); 
    var listOfCloseChallenges = [];

    listOfChallenges.forEach(challenge => 
      { 
          latitude1Rad = challenge.latitude * Math.PI / 180.0;
          longitude1Rad = challenge.longitude * Math.PI / 180.0;
          latititude2Rad = playerLat * Math.PI / 180.0;
          longitude2Rad = playerLng * Math.PI / 180.0;


          //Calculate radians Math.abs()
          logitudeDiff = Math.abs(longitude1Rad - longitude2Rad);
          if (logitudeDiff > Math.PI)
          {
              logitudeDiff = 2.0 * Math.PI - logitudeDiff;
          }
          angleCalculation =
          Math.acos(
              Math.sin(latititude2Rad) * Math.sin(latitude1Rad) +
              Math.cos(latititude2Rad) * Math.cos(latitude1Rad) * 
              Math.cos(logitudeDiff));

          distance = circumference * angleCalculation / (2.0 * Math.PI);

          if ((distance <= maxDistanceFromPlayer) && (challenge.validated))
          {
              //{console.log("Challenge " +challenge._id + " is validated at " + distance + " from player and close enough. Add it!");}
              listOfCloseChallenges.push(challenge);
          }
          /*else {
              if (challenge.validated)
              {console.log("Challenge " +challenge._id + " is validated at " + distance + " from player.");}
              else 
              {console.log("Challenge " +challenge._id + " is not validated at " + distance + " from player.");}
          }*/
      });

    return listOfCloseChallenges;
  }
});


 // Maps to: /api/challengeshunternearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
 Api.addRoute('challengeshunternearby', {
  get: function () {
    var maxDistanceFromPlayer = this.queryParams.maxDistanceFromPlayer;
    var playerLat = this.queryParams.playerLat;
    var playerLng = this.queryParams.playerLng;
    //console.log("maxDistanceFromPlayer: " + maxDistanceFromPlayer); // result: 1234
    //console.log("playerLat: " + playerLat); // result: 10
    //console.log("playerLng: " + playerLng); // result: 10


    var circumference = 40075.0; // Earth's circumference at the equator in km
    var distance = 0.0;
    var latitude1Rad, longitude1Rad, latititude2Rad, longitude2Rad, logitudeDiff, angleCalculation;

    var listOfChallenges = Challenges_HunterDB.find({}); 
    var listOfCloseChallenges = [];

    listOfChallenges.forEach(challenge => 
      { 
          latitude1Rad = challenge.latitude * Math.PI / 180.0;
          longitude1Rad = challenge.longitude * Math.PI / 180.0;
          latititude2Rad = playerLat * Math.PI / 180.0;
          longitude2Rad = playerLng * Math.PI / 180.0;


          //Calculate radians Math.abs()
          logitudeDiff = Math.abs(longitude1Rad - longitude2Rad);
          if (logitudeDiff > Math.PI)
          {
              logitudeDiff = 2.0 * Math.PI - logitudeDiff;
          }
          angleCalculation =
          Math.acos(
              Math.sin(latititude2Rad) * Math.sin(latitude1Rad) +
              Math.cos(latititude2Rad) * Math.cos(latitude1Rad) * 
              Math.cos(logitudeDiff));

          distance = circumference * angleCalculation / (2.0 * Math.PI);

          if ((distance <= maxDistanceFromPlayer) && (challenge.validated))
          {
              //{console.log("Challenge " +challenge._id + " is validated at " + distance + " from player and close enough. Add it!");}
              listOfCloseChallenges.push(challenge);
          }
          /*else {
              if (challenge.validated)
              {console.log("Challenge " +challenge._id + " is validated at " + distance + " from player.");}
              else 
              {console.log("Challenge " +challenge._id + " is not validated at " + distance + " from player.");}
          }*/
      });

    return listOfCloseChallenges;
  }
});


// Maps to: /api/challengesvotingnearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
Api.addRoute('challengesvotingnearby', {
  get: function () {
    var maxDistanceFromPlayer = this.queryParams.maxDistanceFromPlayer;
    var playerLat = this.queryParams.playerLat;
    var playerLng = this.queryParams.playerLng;
    //console.log("maxDistanceFromPlayer: " + maxDistanceFromPlayer); // result: 1234
    //console.log("playerLat: " + playerLat); // result: 10
    //console.log("playerLng: " + playerLng); // result: 10


    var circumference = 40075.0; // Earth's circumference at the equator in km
    var distance = 0.0;
    var latitude1Rad, longitude1Rad, latititude2Rad, longitude2Rad, logitudeDiff, angleCalculation;

    var listOfChallenges = Challenges_VotingDB.find({}); 
    var listOfCloseChallenges = [];

    listOfChallenges.forEach(challenge => 
      { 
          latitude1Rad = challenge.latitude * Math.PI / 180.0;
          longitude1Rad = challenge.longitude * Math.PI / 180.0;
          latititude2Rad = playerLat * Math.PI / 180.0;
          longitude2Rad = playerLng * Math.PI / 180.0;


          //Calculate radians Math.abs()
          logitudeDiff = Math.abs(longitude1Rad - longitude2Rad);
          if (logitudeDiff > Math.PI)
          {
              logitudeDiff = 2.0 * Math.PI - logitudeDiff;
          }
          angleCalculation =
          Math.acos(
              Math.sin(latititude2Rad) * Math.sin(latitude1Rad) +
              Math.cos(latititude2Rad) * Math.cos(latitude1Rad) * 
              Math.cos(logitudeDiff));

          distance = circumference * angleCalculation / (2.0 * Math.PI);

          if ((distance <= maxDistanceFromPlayer) && (challenge.validated))
          {
              //{console.log("Challenge " +challenge._id + " is validated at " + distance + " from player and close enough. Add it!");}
              listOfCloseChallenges.push(challenge);
          }
          /*else {
              if (challenge.validated)
              {console.log("Challenge " +challenge._id + " is validated at " + distance + " from player.");}
              else 
              {console.log("Challenge " +challenge._id + " is not validated at " + distance + " from player.");}
          }*/
      });

    return listOfCloseChallenges;
  }
});


// Challenges_Timer_Multiplayer_IndependentDB
// Maps to: /api/challengestimermultiplayerindependentnearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
Api.addRoute('challengestimermultiplayerindependentnearby', {
  get: function () {
    var maxDistanceFromPlayer = this.queryParams.maxDistanceFromPlayer;
    var playerLat = this.queryParams.playerLat;
    var playerLng = this.queryParams.playerLng;

    var circumference = 40075.0; // Earth's circumference at the equator in km
    var distance = 0.0;
    var latitude1Rad, longitude1Rad, latititude2Rad, longitude2Rad, logitudeDiff, angleCalculation;

    var listOfChallenges = Challenges_Timer_Multiplayer_IndependentDB.find({}); 
    var listOfCloseChallenges = [];

    listOfChallenges.forEach(challenge => 
      { 
          latitude1Rad = challenge.latitude * Math.PI / 180.0;
          longitude1Rad = challenge.longitude * Math.PI / 180.0;
          latititude2Rad = playerLat * Math.PI / 180.0;
          longitude2Rad = playerLng * Math.PI / 180.0;


          //Calculate radians Math.abs()
          logitudeDiff = Math.abs(longitude1Rad - longitude2Rad);
          if (logitudeDiff > Math.PI)
          {
              logitudeDiff = 2.0 * Math.PI - logitudeDiff;
          }
          angleCalculation =
          Math.acos(
              Math.sin(latititude2Rad) * Math.sin(latitude1Rad) +
              Math.cos(latititude2Rad) * Math.cos(latitude1Rad) * 
              Math.cos(logitudeDiff));

          distance = circumference * angleCalculation / (2.0 * Math.PI);

          if ((distance <= maxDistanceFromPlayer) && (challenge.validated))
          {
              listOfCloseChallenges.push(challenge);
          }
      });

    return listOfCloseChallenges;
  }
});



// Challenges_Timed_TaskDB
// Maps to: /api/challengestimedtasknearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
Api.addRoute('challengestimedtasknearby', {
  get: function () {
    var maxDistanceFromPlayer = this.queryParams.maxDistanceFromPlayer;
    var playerLat = this.queryParams.playerLat;
    var playerLng = this.queryParams.playerLng;

    var circumference = 40075.0; // Earth's circumference at the equator in km
    var distance = 0.0;
    var latitude1Rad, longitude1Rad, latititude2Rad, longitude2Rad, logitudeDiff, angleCalculation;

    var listOfChallenges = Challenges_Timed_TaskDB.find({}); 
    var listOfCloseChallenges = [];

    listOfChallenges.forEach(challenge => 
      { 
          latitude1Rad = challenge.latitude * Math.PI / 180.0;
          longitude1Rad = challenge.longitude * Math.PI / 180.0;
          latititude2Rad = playerLat * Math.PI / 180.0;
          longitude2Rad = playerLng * Math.PI / 180.0;


          //Calculate radians Math.abs()
          logitudeDiff = Math.abs(longitude1Rad - longitude2Rad);
          if (logitudeDiff > Math.PI)
          {
              logitudeDiff = 2.0 * Math.PI - logitudeDiff;
          }
          angleCalculation =
          Math.acos(
              Math.sin(latititude2Rad) * Math.sin(latitude1Rad) +
              Math.cos(latititude2Rad) * Math.cos(latitude1Rad) * 
              Math.cos(logitudeDiff));

          distance = circumference * angleCalculation / (2.0 * Math.PI);

          if ((distance <= maxDistanceFromPlayer) && (challenge.validated))
          {
              listOfCloseChallenges.push(challenge);
          }
      });

    return listOfCloseChallenges;
  }
});


// Challenges_Timed_TaskDB
// Maps to: /api/challengesopenquiznearby?maxDistanceFromPlayer=10.0&playerLat=52.0014568&playerLng=4.3280156
Api.addRoute('challengesopenquiznearby', {
  get: function () {
    var maxDistanceFromPlayer = this.queryParams.maxDistanceFromPlayer;
    var playerLat = this.queryParams.playerLat;
    var playerLng = this.queryParams.playerLng;

    var circumference = 40075.0; // Earth's circumference at the equator in km
    var distance = 0.0;
    var latitude1Rad, longitude1Rad, latititude2Rad, longitude2Rad, logitudeDiff, angleCalculation;

    var listOfChallenges = Challenges_OpenQuizDB.find({}); 
    var listOfCloseChallenges = [];

    listOfChallenges.forEach(challenge => 
      { 
          latitude1Rad = challenge.latitude * Math.PI / 180.0;
          longitude1Rad = challenge.longitude * Math.PI / 180.0;
          latititude2Rad = playerLat * Math.PI / 180.0;
          longitude2Rad = playerLng * Math.PI / 180.0;


          //Calculate radians Math.abs()
          logitudeDiff = Math.abs(longitude1Rad - longitude2Rad);
          if (logitudeDiff > Math.PI)
          {
              logitudeDiff = 2.0 * Math.PI - logitudeDiff;
          }
          angleCalculation =
          Math.acos(
              Math.sin(latititude2Rad) * Math.sin(latitude1Rad) +
              Math.cos(latititude2Rad) * Math.cos(latitude1Rad) * 
              Math.cos(logitudeDiff));

          distance = circumference * angleCalculation / (2.0 * Math.PI);

          if ((distance <= maxDistanceFromPlayer) && (challenge.validated))
          {
              listOfCloseChallenges.push(challenge);
          }
      });

    return listOfCloseChallenges;
  }
});



// Maps to: /api/evaluatorRateTeamLastChallengeSeen?TeamID=201805222013493
Api.addRoute('evaluatorRateTeamLastChallengeSeen', {
  get: function () {
      var this_TeamID = this.queryParams.TeamID;
      
      if (this_TeamID)
      {
        
        var listOfLastSeenChallengesByThisTeam = TeamLastAccessedChallengeDB.find({TeamID:this_TeamID});
        
        var found = false;
        var lastSeenChallenge = "";

        listOfLastSeenChallengesByThisTeam.forEach(element => 
          {
            //console.log("This team already saw this challenge:  " + element.LastChallengeIDSeen);
              if (new String(element.TeamID).valueOf() == new String(this_TeamID).valueOf())
              {
                lastSeenChallenge = element.LastChallengeIDSeen;
              }
          });
          
          return lastSeenChallenge;
    }
    return "";
  }
});


// Maps to: /api/gamePermissions?PlayerID=E30E387C456145E0
Api.addRoute('gamePermissions', {
  get: function () {
      var this_PlayerID = this.queryParams.PlayerID;
      
      if (this_PlayerID)
      {
        
        var listOfPermissions = GamePermissionsDB.find({PlayerPlayFabID:this_PlayerID});
        
        var found = false;
        var permissions = 0;

        listOfPermissions.forEach(element => 
          {
            //console.log("This team already saw this challenge:  " + element.LastChallengeIDSeen);
              if (new String(element.PlayerPlayFabID).valueOf() == new String(this_PlayerID).valueOf())
              {
                permissions = element.Permission;
              }
          });
          
          return permissions;
    }
    return 0;
  }
});

// Maps to: /api/entergamePermissions?PlayerID=E30E387C456145E0&DesiredPermission=1
Api.addRoute('entergamePermissions', {
  get: function () {
      var PlayerPlayFabID = this.queryParams.PlayerID;
      var Permission = this.queryParams.DesiredPermission;
      
      if (PlayerPlayFabID && Permission)
      {
            
        var listOfPermissions = GamePermissionsDB.find({PlayerPlayFabID:PlayerPlayFabID});
        
        var found = false;
        //var introduced = false;

        listOfPermissions.forEach(element => 
          {
              var thisID;
              if (!found) // only do it for the first found player
              {
                if (new String(element.PlayerPlayFabID).valueOf() == new String(PlayerPlayFabID).valueOf())
                {
                  console.log("Found the element ID" + element._id);
                  found = true;

                  thisID = element._id;
                  Meteor.call('GamePermissionsDB', thisID, 
                  PlayerPlayFabID, Permission, 
                    (error) => {
                        if (error) {
                            //return "Could not update player last location";
                            console.log("Could not update the permission for " + PlayerPlayFabID + " : " + error);
                            //return false;// "Could not update the permission for " + PlayerPlayFabID +" : " + error;
                            
                            return false;
                        } else {
                            console.log("Permissions for " + PlayerPlayFabID + " were updated to " + Permission);
                            //Session.set(introduced, true);
                            
                            return true;
                        }
                    });
                }
              }

              
          });

          if (!found)
                {
                  console.log("Did not find any challenge in the given team. Adding one for the first time.");
                  
                  GamePermissionsDB.insert({PlayerPlayFabID, Permission});
                  return true;
                }
                else 
                {
                  console.log("The permissions were introduced. ");
                  
                  return true;
                }
          
        }
  
    }
});



// Maps to: /api/teamRankings
Api.addRoute('teamRankings', {
  get: function () {
      var returnList = [];
      var listOfRankings = TeamsRankingDB.find();
      
      listOfRankings.forEach(element => 
        {
          returnList.push(element);
        });
        return returnList;
  }
});

// Maps to: /api/allListOfPermissions
Api.addRoute('allListOfPermissions', {
  get: function () {
      var returnList = [];
      var listOfPermissions = GamePermissionsDB.find();
      
      listOfPermissions.forEach(element => 
        {
          returnList.push(element);
        });
        return returnList;
  }
});


// Maps to: /api/enterTeamRating?TeamID=201805252050088&TeamName=My_darling&TeamRefIcon=TeamIcons/43&Rating=8
Api.addRoute('enterTeamRating', {
  get: function () {
      var TeamID = this.queryParams.TeamID;
      var TeamNameTmp = this.queryParams.TeamName;
      var TeamRefIcon = this.queryParams.TeamRefIcon;
      var Rating = this.queryParams.Rating;
      var TeamName = TeamNameTmp.split("_").join(' ');
      
      if (TeamID && TeamName && TeamRefIcon && Rating)
      {
        if (Rating > 10)
        {
          // if the rating is above what I allow, truncate it to the max (10)
          Rating = 10;
        }
        else if (Rating < 0)
        {
          // if the rating is below what I allow, truncate it to the min (0)
          Rating = 0;
        }
        // ok, now that I have the parameters, I need to see whether this team already has a ranking
        // if so, increment challenges solved and rating
        // if not, introduce anew
            
        var listOfRankings = TeamsRankingDB.find({TeamID:TeamID});
        
        var found = false;
        var introduced = false;

        listOfRankings.forEach(element => 
          {
              var thisID;
              if (!found) // only do it for the first found player
              {
                if (new String(element.TeamID).valueOf() == new String(TeamID).valueOf())
                {
                  found = true;

                  thisID = element._id;
                  Rating = +Rating + +element.Rating
                  // this_TeamID, this_TeamName, this_TeamRefIcon, this_Rating, this_NumberOfChallengesSolved
                  Meteor.call('TeamsRankingDB', thisID, 
                  element.TeamID, element.TeamName, element.TeamRefIcon, Rating, element.NumberOfChallengesSolved + 1,
                    (error) => {
                        if (error) {
                            //return "Could not update player last location";
                            console.log("Could not update the ranking of the team for " + TeamID);
                            return false;
                        } else {
                            console.log("Ranking updated for " + TeamID + " -> " + TeamName);
                            introduced = true;
                        }
                    });
                }
              }

              
          });

          if (!found)
                {
                  var NumberOfChallengesSolved = 1;
                  console.log("Did not find any challenge in the given team. Adding one for the first time.");
                  
                  TeamsRankingDB.insert({TeamID, TeamName, TeamRefIcon, Rating, NumberOfChallengesSolved});
                  return true;
                }
                else 
                {
                  return introduced;
                }
          
        }
  
          
    }
});




}


