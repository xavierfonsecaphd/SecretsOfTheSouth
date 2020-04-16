import { Meteor } from 'meteor/meteor';

Meteor.methods({ 
    sendEmail(text, typeofchallengeindex) {
        var to = 'yourEmail@emailProvider.com';
        var from = 'secretsofthesouth@outlook.com';
        var subject = 'You have a new ';//challenges to validate.';
        switch (typeofchallengeindex) {
            case(0):
                subject = subject.concat('Quiz ');
                break;
            case(1):
                subject = subject.concat('Multiplayer ');
                break;
            case(2):
                subject = subject.concat('Hunter ');
                break;
            case(3):
                subject = subject.concat('Voting ');
                break;
            case(4):
                subject = subject.concat('Timer Multiplayer Independent ');
                break;
            case(5):
                subject = subject.concat('Timed Task ');
                break;
            case(6):
                subject = subject.concat('Open Quiz ');
                break;
            default:
                subject = subject.concat('Undefined ');
                break;
        }
        subject = subject.concat('challenge to validate.');

        // Make sure that all arguments are strings.
        //check([to, from, subject, text], [String]);
    
        // Let other method calls from the same client start running, without
        // waiting for the email sending to complete.
        this.unblock();
    
        Email.send({ to, from, subject, text });
      }
});