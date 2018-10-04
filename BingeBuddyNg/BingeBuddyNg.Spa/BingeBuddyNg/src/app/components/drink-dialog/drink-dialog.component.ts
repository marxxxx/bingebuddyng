import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { DrinkDialogArgs } from './DrinkDialogArgs';
import { DrinkType } from '../../../models/DrinkType';

@Component({
  selector: 'app-drink-dialog',
  templateUrl: './drink-dialog.component.html',
  styleUrls: ['./drink-dialog.component.css']
})
export class DrinkDialogComponent implements OnInit {

  readonly beerGifs = ['https://media.giphy.com/media/92wsX8GEoNTYA/giphy.gif',
    'https://media.giphy.com/media/osbU9PXXgwHuM/giphy.gif',
    'https://media.giphy.com/media/oDgKc9AGIo372/giphy.gif',
    'https://media.giphy.com/media/26gjiYH2vGNjB11wk/giphy.gif',
    'http://static.wixstatic.com/media/ddcd6c_60bf7754b5c502596a79fe039235618b.gif',
    'https://i.pinimg.com/originals/8f/0d/36/8f0d3672ef0b768fdf8fb999c9ca116e.gif'];
  readonly wineGifs = ['https://media1.tenor.com/images/cf4393066940affd7a6cf7bca24d31c4/tenor.gif?itemid=4088025',
    'https://media.giphy.com/media/9U7YUWNqG5j56/giphy.gif'];
  readonly shotGifs = ['https://media1.tenor.com/images/e1981c3c933343bdd610d1a3b89ad26c/tenor.gif?itemid=5499454'];
  readonly antiGifs = ['https://media.tenor.com/images/063a21946b6afea28ca76820acecb5e4/tenor.gif',
    'https://media.giphy.com/media/PAujV4AqViWCA/giphy.gif'];

  imageUrl: string;

  constructor(@Inject(MAT_DIALOG_DATA) public args: DrinkDialogArgs,
    public dialogRef: MatDialogRef<DrinkDialogComponent>) { }

  ngOnInit() {
    this.imageUrl = this.getRandomDrinkImageUrl();
    setTimeout(() => this.dialogRef.close(), 3000);
  }

  onClose() {
    this.dialogRef.close();
  }

  getRandomDrinkImageUrl(): string {
    let gifs: string[] = null;
    switch (this.args.drinkType) {
      case DrinkType.Beer: {
        gifs = this.beerGifs;
        break;
      }
      case DrinkType.Wine: {
        gifs = this.wineGifs;
        break;
      }
      case DrinkType.Shot: {
        gifs = this.shotGifs;
        break;
      }
      case DrinkType.Anti: {
        gifs = this.antiGifs;
        break;
      }
    }

    return this.getRandomLink(gifs);
  }


  getRandomLink(gifs: string[]): string {
    const index = Math.floor(Math.random() * (gifs.length - 1));
    console.log('random index ' + index);
    return gifs[index];
  }

}
