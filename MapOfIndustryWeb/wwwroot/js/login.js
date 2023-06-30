

//axios.defaults.baseURL = 'http://39.99.205.8:5001/';
//axios.defaults.baseURL = 'http://120.24.240.133:5001/';
//axios.defaults.baseURL = 'http://8.129.168.111:5001/';
//axios.defaults.baseURL = 'http://8.142.169.233:5001/';
axios.defaults.baseURL = 'http://localhost:5872/';

function axnormal(data) {

}
function axform() {
    axios.defaults.headers['Content-Type'] = 'application/x-www-form-urlencoded';
}
function axlogin(url, data) {
    return new Promise((resolve, reject) => {
        console.log("axlogin");
        console.log(data);
        url = url + '?' + data;
        console.log(url);
        axios.post(url, data)
            .then(res => {
                console.log(res);
                resolve(res.data)

            })
            .catch(err => {
                console.log(err)
                reject(err.data)
            })
    })
}


