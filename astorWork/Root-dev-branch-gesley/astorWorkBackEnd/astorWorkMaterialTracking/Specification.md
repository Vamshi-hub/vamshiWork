# Material Tracking APIs
## Path
```http
/material_tracking
```
## APIs
1. [List materials](#list-materials)
2. [View material details](#view-material-details)
---
### List materials
#### Endpoint
``` http
GET /projects/{project_id}/materials
```
#### URL params
N.A.
#### Request body
N.A.
#### Response body
**List of following object:**
| Param | Type | Remarks |
| --- | --- | --- |
| ID | integer |  |
| MarkingNo | string |  |
| MaterialType | string |  |
| Block | string |  |
| Level | string |  |
| Zone | string |  |
| Status | string | |
| StatusColor | int | e.g. 0xff0000 |

### View material details
#### Endpoint
``` http
GET /projects/{project_id}/materials/{material_id}
```
#### URL params
N.A.
#### Request body
N.A.
#### Response body
| Param | Type | Remarks |
| --- | --- | --- |
| ID | integer |  |
| MarkingNo | string |  |
| MaterialType | string |  |
| Block | string |  |
| Level | string |  |
| Zone | string |  |
| VendorName | string | |
| TrackerType | string |  |
| TrackerTag | string |  |
| TrackerType | string |  |
| TrackingHistory | Array | List of following objects |

**TrackingHistory**
| Param | Type | Remarks |
| --- | --- | --- |
| ID | integer |  |
| StageName | string | |
| StageStatus | int |  0 - Haven't reached, 1 - Passed this stage, 2 - Failed this stage |
| UpdatedBy | string |   |
| UpdatedDate | DateTime |   |
| Location | string | |

```json
{
  "ID": 1,
  "MarkingNo": "K1F1",
  "Block": "641",
  "Level": "10",
  "Zone": "A",
  "VendorName": "Prefab",
  "TrackerType": "RFID",
  "TrackerTag": "3000E280116060000207A61E706F",
  "TrackingHistory": [
    {
      "ID": 1,
      "StageName": "Produced",
      "StageStatus": 1,
      "UpdatedBy": "vendor - user1",
      "UpdatedDate": "2018-04-01",
      "Location": "Prefab Technology"
    },
    {
      "ID": 2,
      "StageName": "Produced-QA",
      "StageStatus": 1,
      "UpdatedBy": "vendor - user1",
      "UpdatedDate": "2018-04-02",
      "Location": "Prefab Technology"
    }
  ]
}
```

