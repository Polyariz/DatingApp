import { Component, OnInit, Input } from '@angular/core';
import { Message } from '../../_models/message';
import { UserService } from '../../_services/user.service';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService, private authService: AuthService,
      private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
      .pipe(
        tap(messages => {
       
          this.messages = messages as unknown as Message[]; 

          for (let i = 0; i < messages.length; i++) {
            if (this.messages[i].isRead === false && this.messages[i].recipientId === currentUserId) {
              this.userService.markAsRead(currentUserId, this.messages[i].id);
            }
          }
        })
      )      
      .subscribe(messages => { 
        this.messages = messages as unknown as Message[]; 
    }, error => {
      this.alertify.error(error);
    });
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe((message: Message) => {

       // debugger;
        this.messages.unshift(message);
        this.newMessage.content = '';
    }, error => {
      this.alertify.error(error);
    });
  }

}
