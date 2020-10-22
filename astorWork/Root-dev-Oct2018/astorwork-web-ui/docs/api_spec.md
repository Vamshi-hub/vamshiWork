# API Specification (Dev)
This is the specification of all astorWork's APIs.

## Global settings
### Base URL
**pending**
### Timeout
**5** seconds
### Success response
```json
{
    "Status": 0,
    "Data": "..."
}
``` 
### Failed response
```json
{
    "Status": 1,
    "ErrorCode": "TargetExists",
    "ErrorMessage": "The MRF exists in the system"
}
``` 
## List of APIs
[Material Tracking](api_spec_material_tracking.md)