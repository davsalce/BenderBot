/* --- BOT SETTINGS --- */
const botUrl = "https://qwlvr3jf-7054.uks1.devtunnels.ms";
const chatIconMessageEs = "Hola, soy Bender, para ayudarte a trackear tus series he quedado.";
const chatIconMessageEn = "Hi, I'm Bender. My story is a lot like yours, only more interesting ‘cause it involves robots.";

/* --- /BOT SETTINGS --- */

const parameters = {
    style: {
        backgroundColor: '#FFFFFF', //window background color

        bubbleBackground: '#F1F1F4', //bot bubble background color
        bubbleTextColor: '#575A5E', //bot bubble text color
        botAvatarImage: botUrl + '/bender-head.png',
        botAvatarInitials: '', //needed to show image
        botAvatarBackgroundColor: '#194B71',

        bubbleFromUserBackground: '#8A8A8A', //user bubble background color
        bubbleFromUserTextColor: '#ffffff', //user bubble text color

        suggestedActionBackground: 'White', //button background color
        suggestedActionBorderColor: '#cccccc', //button border color
        suggestedActionTextColor: '#194B71', //button text color

        //overlaybutton to move through carousel or suggested actions
        transcriptOverlayButtonBackground: '#d2dde5', //overlaybutton
        transcriptOverlayButtonBackgroundOnHover: '#194B71',
        transcriptOverlayButtonColor: '#194B71',
        transcriptOverlayButtonColorOnHover: 'White', //parameter
    },
    maximize: {
        backgroundColor: '#194B71',
        imageUrl: botUrl + '/bender-head.png',
    },
    header: {
        backgroundColor: '#194B71',
        color: '#194B71',
        imageUrl: botUrl + '/trackseries_logo_transparent_white.png',
        height: '80px'
    },
    directlineTokenUrl: botUrl + '/api/directline/generateToken/',
    directlineReconnectTokenUrl: botUrl + '/api/directline/reconnect/',
    //speechTokenUrl: botUrl + '/api/directline/speech/generatetoken/', //botframework-webchat: "authorizationToken", "region", and "subscriptionKey" are deprecated and will be removed on or after 2020-12-17. Please use "credentials" instead.
    //selectVoice: (voices, activity) => selectVoice(voices, activity),
    chatIconMessage: '',
    language: 'es',
    locale: "es-ES"
}

function addscript(url) {
    var head = document.getElementsByTagName('head')[0];
    var scriptElement = document.createElement('script');
    scriptElement.setAttribute('src', url);

    head.appendChild(scriptElement);
    return scriptElement;
}

var scriptElement = addscript(botUrl + "/main.js");

function addcss(url) {
    var head = document.getElementsByTagName('head')[0];
    var linkElement = document.createElement('link');
    linkElement.setAttribute('rel', 'stylesheet');
    linkElement.setAttribute('type', 'text/css');
    linkElement.setAttribute('href', botUrl + url);

    head.appendChild(linkElement);
}

addcss('/main.css');

function selectVoice(voices, activity) {
    let voice = voices.find(({ lang, gender, name }) => lang.startsWith(activity.locale || parameters.language) && (/AlvaroNeural/iu.test(name) || /GuyNeural/iu.test(name)))
    console.log(voice);
    return voice;
}

scriptElement.onload = function () {
    var scripts = document.getElementsByTagName('script');
    var arrScripts = Array.from(scripts);
    var myScript = arrScripts.find((e) => e.src.includes("botchat.js"));
    var queryString = myScript.src.replace(/^[^\?]+\??/, '');
    var jsParams = new URLSearchParams(queryString);

    if (jsParams.has('locale')) {
        console.log(parameters.language);
        parameters.language = jsParams.get('locale');
    }
    if (parameters.language.startsWith('en')) {
        parameters.chatIconMessage = chatIconMessageEn;
    }
    else {
        parameters.chatIconMessage = chatIconMessageEs;
    }

    window.Intelequia.renderApp('chat-bot', parameters);
};