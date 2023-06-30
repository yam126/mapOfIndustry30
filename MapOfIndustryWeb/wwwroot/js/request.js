function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) != -1) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
function checkCookie() {
    var token1 = getCookie('token');
    console.log("检查了token,值是：" + token1)
    if (token1 == null || token1 == "") {
        window.location = "/Login"
    }
}

//axios.defaults.baseURL = 'http://8.129.168.111:5001/api/v1/';
//axios.defaults.baseURL = 'http://120.24.240.133:5001/api/v1/';
//axios.defaults.baseURL = 'http://39.99.205.8:5001/api/v1/';
axios.defaults.baseURL = 'http://localhost:5872/';
//checkCookie()
//var token = getCookie('token');
//axios.defaults.headers['Authorization'] = 'Bearer ' + token;

function axform() {
    axios.defaults.headers['Content-Type'] = 'application/x-www-form-urlencoded';
}

function axnormal(data) {

}

async function axget(url, params) {
    return new Promise((resolve, reject) => {
        axios.get(url, {
            params: params
        })
            .then(res => {
                resolve(res.data)
            })
            .catch(err => {
                //判断 登录超时
                if (err.toString().indexOf("401") != -1) {

                    window.location = "/Login"
                }
                reject(err)
            })
    })
}

function axpost(url, data) {
    return new Promise((resolve, reject) => {
        axios.post(url, data)
            .then(res => {
                resolve(res)
            })
            .catch(err => {
                //判断 登录超时
                if (err.toString().indexOf("401") != -1) {

                    window.location = "/Login"
                }
                reject(err)
            })
    })
}

function axpostForm(url, data) {
    return new Promise((resolve, reject) => {
        axios.post(url, data)
            .then(res => {
                resolve(res)
            })
            .catch(err => {
                //判断 登录超时
                if (err.toString().indexOf("401") != -1) {
                    window.location = "/Login";
                }
                reject(err)
            })
    })
}

function axput(url, data) {
    return new Promise((resolve, reject) => {
        axios.put(url, data)
            .then(res => {
                resolve(res)
            })
            .catch(err => {
                //判断 登录超时
                if (err.toString().indexOf("401") != -1) {
                    window.location = "/Login"
                }
                reject(err)
            })
    })
}

function axdelete(url, data) {
    return new Promise((resolve, reject) => {
        axios.delete(url, data)
            .then(res => {
                resolve(res)
            })
            .catch(err => {
                //判断 登录超时
                if (err.toString().indexOf("401") != -1) {
                    window.location = "/Login"
                }
                reject(err)
            })
    })
}
