// Server entry point, imports all server code

import '/imports/startup/server';
import '/imports/startup/both';
import { Meteor } from 'meteor/meteor';

import {Inject} from 'meteor/meteorhacks:inject-initial';

//import {Picker} from 'meteor/meteorhacks:picker';

Meteor.startup(() => {

  Inject.rawModHtml('GAInjection', function(html) {
    return html.replace('<head>', '<head><meta charset="utf-8"><meta http-equiv="X-UA-Compatible" content="IE=edge"><meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"><!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries --><!-- WARNING: Respond.js doesn\'t work if you view the page via file:// --><!-- Global site tag (gtag.js) - Google Analytics --><script async src="https://www.googletagmanager.com/gtag/js?id=UA-118716978-1"></script><script>window.dataLayer = window.dataLayer || [];function gtag(){dataLayer.push(arguments);}gtag(\'js\', new Date());gtag(\'config\', \'UA-118716978-1\');</script><meta name="google-site-verification" content="43fzPV1Cwlpyz0d3-Ob3JDJeiMA4ab8_nPWSAs1_x4k" />');
  });

  //process.env.MAIL_URL = 'smtps://noreply_secretsofthesouth%40yahoo.com:SecretsOfTheSouth_sots0@smtp.mail.yahoo.com:465';//'smtp://0.0.0.0:1025';
  process.env.MAIL_URL = 'smtp://secretsofthesouth%40outlook.com:SotS_noreply@smtp.live.com:25';//'smtp://0.0.0.0:1025';
  // secretsofthesouth@outlook.com
    // code to run on server at startup
    // console.log(Meteor.settings.hello);

// if the Links collection is empty
/*
if (Challenges.find().count() === 0) {
  const data = [
    {
      name: 'Example',
      description: 'Example Description',
      ownerPlayFabID: 'invalidID',
      typeOfChallengeIndex: 0,
      numberOfPeopleToMeet: 2,
      latitude: '52.123343212',
      longitude: '-4.123343212',
      question: 'Vinho verde?',
      answer: 'Branco',
      imageURL: 'http://sapo.pt/img.jpg',
    }
  ];

    data.forEach(challenge => Challenges.insert(challenge));
    console.log("Initiated DB of Challenges");
    }
*/
  });
/*
  Picker.route('/', (params, req, res, next) => {
    const head =
      `
      <meta charset="utf-8">
      <meta http-equiv="X-UA-Compatible" content="IE=edge">
      <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">
      
      <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
      <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
      <!-- Global site tag (gtag.js) - Google Analytics -->
      <script async src="https://www.googletagmanager.com/gtag/js?id=UA-118716978-1"></script>
      <script>
        window.dataLayer = window.dataLayer || [];
        function gtag(){dataLayer.push(arguments);}
        gtag('js', new Date());

        gtag('config', 'UA-118716978-1');
      </script>
      <meta name="google-site-verification" content="43fzPV1Cwlpyz0d3-Ob3JDJeiMA4ab8_nPWSAs1_x4k" />
      `;
      
    injectHead(head, res);		
    
    next();
  });*/