const path = require('path')
const jsonServer = require('json-server')

const uuid = require('uuid');
const nJwt = require('njwt');
//const middlewares = require('./middleware')
/*
var server = jsonServer.create()
//var rewriter = jsonServer.rewriter('routes.json');
var router = jsonServer.router('data.json')

server.use(jsonServer.defaults)
//server.use(rewriter)
server.use(router)

server.listen(4201, () => {
  console.log('JSON Server is running')
})
*/
const server = jsonServer.create()
const router = jsonServer.router(path.join(__dirname, 'data.json'))
const middlewares = jsonServer.defaults()

router.render = function (req, res) {
  result = {
    status: 0,
    data: res.locals.data
  }
  if (require('url').parse(req.originalUrl, true).query['singular']) {
    if (Array.isArray(result.data)) {
      result.data = result.data[0]
    }
  }
  res.jsonp(result)
}

server.use(middlewares)
server.use(jsonServer.rewriter(
  {
    "/material-tracking/*": "/$1",
    "/projects/:projectId/materials/:materialId": "/materialDetails/:materialId",
    "/projects/:projectId/mrfs/create": "/projects/:projectId/mrfs",
    "/projects/:projectId/mrfs/location?block=:blk": "/projects/:projectId/mrfLocations?block=:blk",
    "/projects/:projectId/mrfs/material?block=:blk&level=:lvl&zone=:zone": "/projects/:projectId/mrfComponents?block=:blk&level=:lvl&zone=:zone",
    "/projects/:projectId/vendors/:vendorId/inventory": "/projects/:projectId/inventory?vendorId=:vendorId",
    "/projects/:projectId/vendors/:vendorId/inventory/pre_create?materialType=:materialType": "/projects/:projectId/inventory_pre_create?vendorId=:vendorId&materialType=:materialType",
    "/trackers/association?tags=:tag": "/trackers?tags=:tag",
    "/materials/:materialId/nextStage?singular=:singular": "/materialNextStage?materialId=:materialId&singular=:singular",
    "/projects/:projectId/dashboard/stats": "/projects/:projectId/materialStageAuditStats",
    "/projects/:projectId/dashboard/progress?block=:blk": "/projects/:projectId/materialsProgress",
    "/projects/:projectId/dashboard/qc-open-and-daily-status": "/projects/:projectId/materialStageAuditQcOpenAndDailyStatus",
    "/role/default-pages": "/defaultpages",
    "/role/pages": "/pages",
    "/qc/defect?case_id=:case_id": "/qcDefect?qcCaseId=:case_id",
    "/qc/case?stage_audit_id=:stage_audit_id": "/qcCase?stageAuditId=:stage_audit_id",
    "/sites/countries":"/countries"
  }
))

server.use(jsonServer.bodyParser)

server.post('/projects/:projectId/mrfs', (req, res) => {
  res.jsonp({
    status: 0,
    data: {
      id: 100,
      mrfNo: "MRF-2018-0005",
      materialCount: 10,
    }
  })
})

server.put('/projects/:projectId/materials/edit/:materialId', (req, res) => {
  res.jsonp({
    status: 0
  })
})

server.post('/projects/:projectId/vendors/:vendorId/inventory', (req, res) => {
  res.jsonp({
    status: 0
  })
})

server.post('/vendors/:vendorId/afterInventory', (req, res) => {
  res.jsonp({
    status: 0
  })
})

server.post('/materials/:materialId/updateStage', (req, res) => {
  res.jsonp({
    status: 0,
    data: {
      id: 1
    }
  })
})

server.post('/upload-qc-photo/:stageAuditId', (req, res) => {
  res.jsonp({
    status: 0
  })
})

server.get('/trackers?tags=:tags', (req, res) => {
  res.jsonp({
    status: 0
  })
})

var claims = {
  "sub": "7",
  "jti": "be1b2ce9-0972-48cb-b807-a63408164ce9",
  "iat": "06/25/2018 05:51:23",
  "personName": "Admin",
  "vendorId": "0",
  "tenantName": "tenant1",
  "nbf": 1529905883,
  "exp": 1529909483,
  "iss": "astorwork",
  "aud": "astorwork",
  "role": {
    "userId": 7,
    "expiryTime": "2018-06-25T06:51:23.7780628+00:00",
    "pageAccessRights": [
      {
        "url": "/material-tracking/bim-syncs",
        "right": 3
      },
      {
        "url": "/material-tracking/materials/:id",
        "right": 3
      },
      {
        "url": "/material-tracking/mrfs",
        "right": 3
      },
      {
        "url": "/material-tracking/mrfs/create",
        "right": 3
      },
      {
        "url": "/material-tracking/bim-syncs/:id",
        "right": 3
      },
      {
        "url": "/configuration/role-master",
        "right": 3
      },
      {
        "url": "/user-account/:id",
        "right": 3
      },
      {
        "url": "/user-account/change-password",
        "right": 3
      },
      {
        "url": "/configuration/user-master",
        "right": 3
      },
      {
        "url": "/configuration/user-details/:id",
        "right": 3
      },
      {
        "url": "/configuration/role-details/:id",
        "right": 3
      },
      {
        "url": "/material-tracking/dashboard",
        "right": 3
      },
      {
        "url": "/material-tracking/materials",
        "right": 3
      },
      {
        "url": "/configuration/vendor-master",
        "right": 3
      },
      {
        "url": "/configuration/vendor-details/:id",
        "right": 3
      },
      {
        "url": "/configuration/location-master",
        "right": 3
      },
      {
        "url": "/configuration/location-details/:id",
        "right": 3
      },
      {
        "url": "/configuration/stage-master",
        "right": 3
      },
      {
        "url": "/configuration/stage-master/create",
        "right": 3
      },
      {
        "url": "/configuration/site-master",
        "right": 3
      },
      {
        "url": "/configuration/site-details/:id",
        "right": 3
      },
      {
        "url": "/material-tracking/qc-cases/:id",
        "right": 3
      },
      {
        "url": "/material-tracking/qc-defects/:id",
        "right": 3
      },
      {
        "url": "/configuration/generate-qr-code",
        "right": 3
      },
      {
        "url": "/configuration/notification-config",
        "right": 3
      }
    ],
    "defaultPage": "/material-tracking/dashboard",
    "mobileEntryPoint": 1
  }
}

var jwt = nJwt.create(claims, "test12345", "HS256");
const token = jwt.compact();

server.post('/authentication/login', (req, res) => {
  res.jsonp({
    status: 0,
    data: {
      tokenType: "Bearer",
      expiresIn: 3600,
      accessToken: token,
      userId: "1"
    }
  })
})

server.post('/authentication/refresh', (req, res) => {
  res.jsonp({
    status: 0,
    data: {
      tokenType: "Bearer",
      expiresIn: 3600,
      accessToken: token,
      userId: "1"
    }
  })
})

server.get('/user', (req, res) => {
  res.jsonp({
    status: 0
  })
})

server.use(router)
server.listen(4201, () => {
  console.log('JSON Server is running')
})