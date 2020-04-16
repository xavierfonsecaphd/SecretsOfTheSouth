// Meteor.subscribe('challenges');  
import { ChallengesDB } from '/Collections/Challenges.js';
import { Meteor } from 'meteor/meteor';
import './Challenge.html';
import { GoogleMaps } from 'meteor/dburles:google-maps';



//console.log(Meteor.settings.public.ga.account);

Template.Challenge.onCreated (function () {
    var self = this;
    self.autorun (function() {
        var id = FlowRouter.getParam('_id');
        self.subscribe('challenge', id);
    });
});





Template.Challenge.helpers({ 
    // Basicamente isto é um helper que vai colocar todas as receitas da BD dentro de "recipes"
    challenge: () => {
        var id = FlowRouter.getParam('_id');
        return ChallengesDB.findOne({_id: id});
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
            return 'AR';
        }
        else if (index == 2)
        {
            return 'IoT';
        }
        else { return 'Other';}
    }
}); 


Template.Challenge.events ({ 
    'submit .info-challenge-edit'(event) {
        event.preventDefault();
        const target = event.target;
    
        const name = target.challenge_name;
        const description = target.description;
        const route = target.route;
        if ((route.value != 0) && (route.value != 1) && (route.value != 2) && (route.value != 3)) {route.value = 0;}

        const typeOfChallengeIndex = target.typeOfChallengeIndex;
        const numberOfPeopleToMeet = target.numberOfPeopleToMeet;
        const latitude = target.latitude;
        const longitude = target.longitude;
        const question = target.question;
        const answer = target.answer;
        const imageURL = target.imageURL;

        const ownerPlayFabID = Meteor.user().username;
        console.info('user id: ' + ownerPlayFabID);
        console.info('name: ' + name);
        console.info('description: ' + description);
        console.info('latitude: ' + latitude);
        console.info('longitude: ' + longitude);

        if (!!ownerPlayFabID)
        {

            Meteor.call('updateChallenge', this._id, 
            name.value, 
            description.value,
            ownerPlayFabID,
            typeOfChallengeIndex.value,
            numberOfPeopleToMeet.value,
            latitude.value,
            longitude.value,
            question.value,
            answer.value,
            imageURL.value,
            route.value,
            false, (error) => {
                if (error) {
                    alert(error.error);
                } else {
                    
                    alert("Challenge update successfully :)");
                }
            });

        }
        else
        {
            FlowRouter.go('/');
        }
    
        
      }
});

