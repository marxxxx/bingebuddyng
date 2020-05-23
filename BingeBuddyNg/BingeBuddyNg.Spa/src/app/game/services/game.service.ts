import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { StartGameResultDTO } from '../models/StartGameResultDTO';
import { StartGameDTO } from '../models/StartGameDTO';
import { Observable } from 'rxjs';
import { AddGameEventDTO } from '../models/AddGameEventDTO';
import { GameDTO } from '../models/GameDTO';

@Injectable({
  providedIn: 'root'
})
export class GameService {

  baseUrl = environment.BaseDataUrl + '/game';

  constructor(private http: HttpClient) { }

  startGame(game: StartGameDTO): Observable<StartGameResultDTO> {
    const url = `${this.baseUrl}/start`;
    return this.http.post<StartGameResultDTO>(url, game);
  }

  addGameEvent(gameId: string, gameEvent: AddGameEventDTO): Observable<void> {
    const url = `${this.baseUrl}/${gameId}/event`;
    return this.http.post<void>(url, gameEvent);
  }

  getGame(gameId: string): Observable<GameDTO> {
    const url = `${this.baseUrl}/${gameId}`;
    return this.http.get<GameDTO>(url);
  }
}
