import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';

export const SotSEventsDB = new Mongo.Collection('SotSEvents'); 

// not used at this point
Meteor.methods({
    'SotSEventsDB.insert'(name, playfabid, latitude, longitude, timestamp, message) {

/*
var name = req.body["name"];
  var playfabid = req.body["playfabid"];
  var latitude = req.body["latitude"];
  var longitude = req.body["longitude"];
  var timestamp = req.body["timestamp"];
  var message = req.body["message"]; */
    
        return SotSEventsDB.insert({name, playfabid, latitude, longitude, timestamp, message});
    }/*,
      UpdatePlayerTeam: function (id, this_PlayerPlayFabID, this_TeamID, this_TeamName, this_TeamRefIcon) {
        TeamsDB.update(id, {
            $set: {
                PlayerPlayFabID: this_PlayerPlayFabID,
                TeamID: this_TeamID,
                TeamName: this_TeamName,
                TeamRefIcon: this_TeamRefIcon
            }
        });
      }*/
});