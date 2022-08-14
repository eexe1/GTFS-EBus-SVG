[![codecov](https://codecov.io/gh/eexe1/GTFS-EBus-SVG/branch/main/graph/badge.svg?token=LELOKIV42K)](https://codecov.io/gh/eexe1/GTFS-EBus-SVG)

# Introduction

GTFS-EBus-SVG is built for the Saint Vincent and the Grenadines E-Bus system. It brings the real-time bus information to the Google Transit. Therefore, users on Google Map can see the real-time arrival for buses. The application is built in .Net Core 3.1 and runs on Google Cloud Function.

Data source comes WebStopInfo API from *https://ebus.gov.vc* . The update interval is expected to be 30 seconds.

# API Access

To access the API, you need to initiate a GET call on `https://{google_cloud_function_path}`.

Possible parameters are _route_ and _type_.

_route_: Optional, default is _all_

- _windward_
- _leeward_
- _all_

_type_: Optional, default is TripUpdate

- _vehicle_ : For VehiclePosition


# Data Explanation

To understand the data, you must first obtain the knowledge of the E-Bus system data format.
WebStopInfo API: *https://ebus.gov.vc/stopapi/webstopinfo?rid={id}* where Windward's side id is 4, and Leeward, 5.
Real-time data is availabe from the above API, and it consists of an array of the below json object.

```json
{
  "id": 4,
  "nm": "Envy",
  "enm": "Envy",
  "dir": 0,
  "seq": 72,
  "est": "3m21s",
  "cnt": 6,
  "bno": [
    {
      "no": "HD452",
      "lat": 13.133655,
      "lon": -61.200336666666665,
      "tm": 1655145770,
      "sid": 85,
      "seq": 66,
      "alias": "Mini Bus HD452"
    }
  ]
}
```

_id_ is the route. _nm_, _enm_ are the bus stop name. _seq_ is the bus stop sequence number. _bno_ contains the bus current information including _no_ bus plate number, latitude, longitude, _sid_ near bus stop id, _seq_ near bus stop sequence number.
