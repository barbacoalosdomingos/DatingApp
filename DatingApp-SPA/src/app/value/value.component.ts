import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {
  // Declarar la propiedad values, que sera de cualquier tipo , algo asi como un var.
  values: any;

  // Debemos usar el httpclient importando desde @angular/common/http.
  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getValues();
  }
// Funcion que hace una peticion http.
  getValues() {
    // Los parametros de la funcion get son, la url y los objetos que mandaremos o no.
    // Se tienen que poner el subscribe, que sirve para recibir una respuesta , y hacer algo
    // con esos valores que nos regreso.
    this.http.get('http://localhost:5000/api/values').subscribe(response => {
      this.values = response;
    }, error => {
      console.log(error);
    });
  }

}
