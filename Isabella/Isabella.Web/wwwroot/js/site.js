//Datos del servidor.
let hosting = true;
let URI_SERVER;

function getUriServer()
{
    if (hosting) {
        //Hosting
        URI_SERVER = 'http://jrariasga-001-site1.btempurl.com/';
    }
    else {
        //Local
        URI_SERVER = 'https://localhost:44352/';
    }
    return URI_SERVER;
}



