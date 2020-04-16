// Client entry point, imports all client code

import '/imports/startup/client';
import '/imports/startup/both';
import '/imports/startup/accounts-config.js';


Meteor.startup(function() {
    GoogleMaps.load({key: 'AIzaSyCHeR5WwL4vj0hadx56ZTqa6LV8YG1MVdk'});
  });