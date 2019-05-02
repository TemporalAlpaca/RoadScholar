import { Component, Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { forEach } from '@angular/router/src/utils/collection';
import { resolve } from 'url';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.css'],
})

@Injectable()
export class AppComponent {
  lat: number = 44.047349598258734;
  lng: number = -121.31899318657815;
  fileCount: number;
  tempRoute: Route;
  routes: Route[];
  points: Point[];

  constructor(private http: HttpClient) {
    var i;

    this.http.get('https://localhost:5001/api/upload', { observe: 'response' })
      .subscribe(response => {
        this.fileCount = Number(response.body);
        console.log("Number of files: " + this.fileCount);

        //Create list of routes
        this.routes = [] as Array<Route>

        //Get all of the JSON files in the directory
        for (i = 0; i < this.fileCount; i++) {
          console.log("For loop: " + i);
          this.getJSON(i + 1).subscribe(data => {
            console.log(data);
            this.points = JSON.parse(JSON.stringify(data));
            //Create temporary route to add to list
            this.tempRoute = {} as Route;
            this.tempRoute.points = this.points;
            console.log(this.tempRoute);
            //Add route to route list
            this.routes.unshift(this.tempRoute);
            console.log(this.routes);
          });
        }
      });
  }

  public getJSON(index): Observable<any> {
    console.log("Get JSON: " + index);
    return this.http.get("./assets/JsonPaths/Paths" + index +".json");
  }
}

interface Point {
  latitude: number;
  longitude: number;
  timeStamp: number;
  distance: number;
  speed: number;
  heartRate: number;
}

interface Route {
  points: Point[];
}
