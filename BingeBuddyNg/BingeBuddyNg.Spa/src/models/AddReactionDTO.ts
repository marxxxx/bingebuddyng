import { ReactionType } from './ReactionType';

export class AddReactionDTO {
    type: ReactionType;
    activityId: string;
    comment?: string;
}
