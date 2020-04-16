import { Meteor } from 'meteor/meteor';

Template.QRCode.onCreated (function () {
  
console.log('Passed parameter: '+this.params.id);
    

    let array = new Uint32Array(8)
    window.crypto.getRandomValues(array)
    let str = ''
    for (let i = 0; i < array.length; i++) {
      str += (i < 2 || i > 5 ? '' : '-') + array[i].toString(16).slice(-4)
    }
    
    this.url = new ReactiveVar ('http://secretsofthesouth.tbm.tudelft.nl/HandleQRCode/_' + str);

  });
  
  
  Template.QRCode.helpers({ 
    
    
    url: function () {
      return Template.instance().url.get();
    }
  }); 