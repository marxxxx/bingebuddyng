import { ReactionType } from "./ReactionType";

export class ReactionDTO {
    type: ReactionType;
    activityId: string;
    comment?: string;
}
