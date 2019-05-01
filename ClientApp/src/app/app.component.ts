import { Component, Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { forEach } from '@angular/router/src/utils/collection';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.css'],
})

@Injectable()
export class AppComponent {
  lat: number = 44.047349598258734;
  lng: number = -121.31899318657815;
  points: Point[];

  constructor(private http: HttpClient) {
    this.getJSON().subscribe(data => {
      console.log(data);
      this.points = JSON.parse(JSON.stringify(data));
    });
  }

  public getJSON(): Observable<any> {
    return this.http.get("./assets/Paths.json");
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
