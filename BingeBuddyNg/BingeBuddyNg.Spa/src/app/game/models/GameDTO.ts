import { GameStatus } from './GameStatus';
import { UserScoreInfoDTO } from './UserScoreInfoDTO';

export interface GameDTO {
  id: string;

  title: string;

  userScores: UserScoreInfoDTO[];

  status: GameStatus;
}
