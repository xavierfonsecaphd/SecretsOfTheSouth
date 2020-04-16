import { Mongo } from 'meteor/mongo';
import { Meteor } from 'meteor/meteor';
import { check } from 'meteor/check';


export const Markers = new Mongo.Collection('markers');



Meteor.methods({
    'markers.insert'(_lat, _lng) {
        
        
    
        return Markers.insert({ lat: _lat, lng: _lng });
      },
    deleteMarker: function (id) {
        Markers.remove(id);
        },
      updateMarker: function (id, _lat, _lng) {
          Markers.update(id, {
              $set: {
                    lat: _lat,
                  lng: _lng
              }
          });
      }
      
});