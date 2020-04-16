import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';

// permission 0 -> no record
// permission 1 -> evaluation
// permission 2 -> observation
// permission 3 -> Administration   [E30E387C456145E0]
// permission 4 -> evaluation, observation
// permission 5 -> evaluation, Administration
// permission 6 -> observation, Administration
// permission 7 -> evaluation, observation, Administration



export const GamePermissionsDB = new Mongo.Collection('var_GamePermissions'); 

// not used at this point
Meteor.methods({
    'GamePermissionsDB.insert'(PlayerPlayFabID, Permission) {
        check(PlayerPlayFabID, String);
    
        return GamePermissionsDB.insert({ PlayerPlayFabID, Permission});
      },
      GamePermissionsDB: function (id, this_PlayerPlayFabID, this_Permission) {
        GamePermissionsDB.update(id, {
            $set: {
                PlayerPlayFabID: this_PlayerPlayFabID,
                Permission: this_Permission
            }
        });
    }
});


