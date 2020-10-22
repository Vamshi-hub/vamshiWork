'use strict'

module.exports = (req, res, next) => {
    const _send = res.send
    res.send = function (body) {
        if (require('url').parse(req.originalUrl, true).query['singular']) {
            try {
                const bodyJson = JSON.parse(body)
                cnsole.log(bodyJson.data)
                const json = JSON.parse(bodyJson.data)
                if (Array.isArray(json)) {
                    if (json.length >= 1) {
                        return _send.call(this, {
                            status: 0,
                            data: json[0]
                        })
                    } else if (json.length === 0) {
                        return _send.call(this, {}, 404)
                    }
                }
            } catch (e) {
                return _send.call(this, {
                    status: 1,
                    message: e.message
                }, 500)
            }
        }
        return _send.call(this, body)
    }
    next()
}