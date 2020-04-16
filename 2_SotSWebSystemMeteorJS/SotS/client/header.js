import { Template } from 'meteor/templating';
import {Inject} from 'meteor/meteorhacks:inject-initial';
import {Injected} from 'meteor/meteorhacks:inject-initial';
import { Meteor } from 'meteor/meteor';

// The Header menu does not use dropdown menus, but most menus do.
// Here's how to do the required initialization for Semantic UI dropdown menus.
Template.Header.onRendered(function enableDropDown() {
  this.$('.dropdown').dropdown();

  //Injected.rawModHtml('GAInjection');

});


// == DC5F625A22855648

Template.Header.onCreated (function () {
  

  var self = this;
  
  self.autorun (function() {
      self.subscribe('challenges');
      self.subscribe('challenges1');
      self.subscribe('Challenges_Hunter');
      self.subscribe('Challenges_Voting');
      self.subscribe('Challenges_Timer_Multiplayer_Independent');
      self.subscribe('Challenges_Timed_Task');
      self.subscribe('Challenges_OpenQuiz');
      self.subscribe('SotSEvents'); // import { SotSEventsDB } from '/Collections/SotSEvents.js';
  });
});


Template.Header.helpers({ 
  
  // porque estás a usar variáveis, precisas de as colocar no helper
  isAdmin: function () {
    return Meteor.user().username == 'administrator';
                                                  // from this template
  }
}); 