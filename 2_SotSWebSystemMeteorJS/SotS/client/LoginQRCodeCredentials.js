// LoginQRCodeCredentials
import { Meteor } from 'meteor/meteor';

Template.LoginQRCodeCredentials.onCreated (function () {
  

    //console.log('Passed parameter: '+FlowRouter.getParam('_id'));
    
    this.url = new ReactiveVar ('http://secretsofthesouth.tbm.tudelft.nl/HandleQRCode/_' + 
                FlowRouter.getParam('_id'));

    this.playerID = new ReactiveVar (FlowRouter.getParam('_id'));
  });
  
  
  Template.LoginQRCodeCredentials.helpers({ 
    playerID() {
        return Template.instance().playerID.get();
    },
    
    url: function () {
      return Template.instance().url.get();
    }
  }); 